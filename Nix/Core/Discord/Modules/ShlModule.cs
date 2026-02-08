using Discord.Commands;
using Nix.Core.Shl.Overview;
using Nix.Core.Shl.Standings;
using Nix.Core.Shl.Teams;
using Nix.Infrastructure;
using Nix.Rendering;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nix.Core.Discord.Modules;

[Group("shl")]
public class ShlModule : ModuleBase<SocketCommandContext>
{
    private readonly IRenderEngine renderer;
    private readonly IOverviewService overviewService;
    private readonly IStandingsService standingsService;
    private readonly ITeamService teamService;
    private readonly string css;
    private readonly string matchCardTemplate;
    private readonly string standingsTemplate;
    private readonly string standingsTeamTemplate;
    private readonly string liveStatus;
    private readonly string endedStatus;

    public ShlModule(IRenderEngine renderer, IOverviewService overviewService, IStandingsService standingsService, ITeamService teamService)
    {
        this.renderer = renderer;
        this.overviewService = overviewService;
        this.standingsService = standingsService;
        this.teamService = teamService;

        css = File.ReadAllText(NixConstants.StylingPath);
        matchCardTemplate = File.ReadAllText(NixConstants.MatchCardTemplatePath);
        standingsTemplate = File.ReadAllText(NixConstants.StandingsTemplatePath);
        standingsTeamTemplate = File.ReadAllText(NixConstants.StandingsTeamTemplatePath);
        liveStatus = File.ReadAllText(NixConstants.LiveStatusPath);
        endedStatus = File.ReadAllText(NixConstants.EndedStatusPath);
    }

    [Command("upcoming")]
    [Summary("Lists the upcoming games.")]
    public async Task ListShlTeamsAsync()
    {
        var timer = Stopwatch.StartNew();
        var gameInfo = await overviewService.GetGameInfoAsync();
        var upcoming = gameInfo.Matches.Where(m => m.StartDateTime.Date >= DateTime.UtcNow);
        var earliest = upcoming.Min(m => m.StartDateTime);
        var matches = upcoming.Where(m => m.StartDateTime.Date == earliest.Date);


        StringBuilder builder = new StringBuilder();
        builder.Append("<div class=\"matches\">");

        var html = await Task.WhenAll(matches.Select(CreateMatchCardAsync));
        builder.AppendJoin(string.Empty, html);

        builder.Append("</div>");
        var streams = await renderer.RenderManyAsync(builder.ToString(), css, ".match-card", nameof(ListShlTeamsAsync));

        var semaphore = new SemaphoreSlim(3);
        var tasks = streams.Select(async s =>
        {
            await semaphore.WaitAsync();
            await Context.Channel.SendFileAsync(s, $"{Guid.NewGuid}.png");
            await Task.Delay(1000);
            semaphore.Release();
        });

        await Task.WhenAll(tasks);
        timer.Stop();
        await Context.Channel.SendMessageAsync($"{timer.Elapsed.TotalMilliseconds}ms");
    }

    [Command("standings")]
    [Summary("Lists the team standings.")]
    public async Task StandingsAsync()
    {
        var overview = await overviewService.GetGameInfoAsync();
        var standings = await standingsService.GetStandingsAsync(overview.Id);
        var teams = await Task.WhenAll(standings.Select(s => teamService.GetTeamAsync(s.Id)));
        var chunks = teams.Zip(standings, (team, standing) => (Team: team, Standing: standing)).Chunk(teams.Length / 2);

        var tasks = chunks.Select(c =>
        {
            var rows = c.Select(c => CreateStandingsTeam(c.Standing, c.Team));
            var html = CreateStandings(string.Join(string.Empty, rows));
            return renderer.RenderSingleAsync(html, css, ".standings-card", nameof(StandingsAsync));
        });

        var streams = await Task.WhenAll(tasks);

        foreach (var stream in streams)
        {
            await Context.Channel.SendFileAsync(stream, $"{Guid.NewGuid}.png");
        }
    }

    private async Task<string> CreateMatchCardAsync(Match match)
    {
        var homeTask = teamService.GetTeamAsync(match.HomeTeam.Id);
        var awayTask = teamService.GetTeamAsync(match.AwayTeam.Id);
        await Task.WhenAll(homeTask, awayTask);
        var home = await homeTask;
        var away = await awayTask;

        var homeHex = home.HexTeamColors();
        var awayHex = away.HexTeamColors();

        return matchCardTemplate
            .Replace("{HOME_TEAM_NAME}", home.LongName)
            .Replace("{HOME_TEAM_LOGO}", home.IconUri.AbsoluteUri)
            .Replace("{AWAY_TEAM_NAME}", away.LongName)
            .Replace("{AWAY_TEAM_LOGO}", away.IconUri.AbsoluteUri)
            .Replace("{HOME_HEX_1}", homeHex[0])
            .Replace("{HOME_HEX_2}", homeHex[1])
            .Replace("{AWAY_HEX_1}", awayHex[0])
            .Replace("{AWAY_HEX_2}", awayHex[1])
            .Replace("{STATUS}", GetStatusHtml(match))
            .Replace("{DATE}", GetDateHtml(match))
            .Replace("{MATCH_TIME}", match.StartDateTime.ToLocalTime().ToShortTimeString())
            .Replace("{ARENA_NAME}", match.Venue);
    }

    private string CreateStandings(string html) =>
        standingsTemplate.Replace("{BODY}", html);

    private string CreateStandingsTeam(StandingsTeam standingsTeam, Team team)
    {
        var hex = team.HexTeamColors();
        return standingsTeamTemplate
            .Replace("{TEAM_HEX_1}", hex[0])
            .Replace("{TEAM_HEX_2}", hex[1])
            .Replace("{RANK}", standingsTeam.Rank.ToString())
            .Replace("{TEAM_LOGO}", team.IconUri.AbsoluteUri)
            .Replace("{TEAM_NAME}", team.LongName)
            .Replace("{GP}", standingsTeam.GamesPlayed.ToString())
            .Replace("{POINTS}", standingsTeam.Points.ToString());
    }

    private string GetStatusHtml(Match match)
    {
        if (match.IsPlaying())
        {
            return liveStatus;
        }

        if (match.HasEnded())
        {
            return endedStatus;
        }

        return string.Empty;
    }

    private static string GetDateHtml(Match match)
    {
        if (match.HasStarted() || match.HasEnded())
        {
            return string.Empty;
        }

        if (match.StartDateTime.Date == DateTime.UtcNow.Date)
        {
            return "Today";
        }

        if (match.StartDateTime.Date == DateTime.UtcNow.Date.AddDays(1))
        {
            return "Tomorrow";
        }

        return match.StartDateTime.ToString("dddd dd MMMM");
    }
}
