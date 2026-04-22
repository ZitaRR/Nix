using Discord.Commands;
using Nix.Core.Shl;
using Nix.Core.Shl.Overview;
using Nix.Core.Shl.Standings;
using Nix.Core.Shl.Teams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Nix.Core.Discord.Modules;

[Group("shl")]
public class ShlModule(
    ShlContext shlContext,
    IOverviewService overviewService, 
    IStandingsService standingsService, 
    ITeamService teamService) : ModuleBase<SocketCommandContext>
{

    [Command("upcoming")]
    [Summary("Lists the upcoming games.")]
    public async Task ListShlTeamsAsync()
    {
        var timer = Stopwatch.StartNew();
        var season = await shlContext.GetCurrentSeasonAsync();
        var gameType = await shlContext.GetRegularSeasonAsync();
        var gameInfo = await overviewService.GetGameInfoAsync(season, gameType);
        var matches = await GetNextMatchesAsync(gameInfo.Matches);

        if (!matches.Any())
        {
            gameType = await shlContext.GetRelegationAsync();
            gameInfo = await overviewService.GetGameInfoAsync(season, gameType);
            matches = await GetNextMatchesAsync(gameInfo.Matches);
        }

        if (!matches.Any())
        {
            gameType = await shlContext.GetPlayoffsAsync();
            gameInfo = await overviewService.GetGameInfoAsync(season, gameType);
            matches = await GetNextMatchesAsync(gameInfo.Matches);
        }

        if (!matches.Any())
        {
            await ReplyAsync("Season has ended!");
            return;
        }
        
        var versus = await Task.WhenAll(matches.Select(GetTeamsForMatchAsync));

        var message = string.Join("\n", versus);
        await ReplyAsync(message);

        timer.Stop();
        await Context.Channel.SendMessageAsync($"{timer.Elapsed.TotalMilliseconds}ms");
    }

    [Command("standings")]
    [Summary("Lists the team standings.")]
    public async Task StandingsAsync()
    {
        var timer = Stopwatch.StartNew();
        var season = await shlContext.GetCurrentSeasonAsync();
        var gameType = await shlContext.GetRegularSeasonAsync();
        var gameInfo = await overviewService.GetGameInfoAsync(season, gameType);
        var standings = await standingsService.GetStandingsAsync(gameInfo.Id);
        var teams = await Task.WhenAll(standings.Select(s => teamService.GetTeamAsync(s.Id)));
        var teamStandings = teams.Zip(standings, (team, standing) => (Team: team, Standing: standing));

        var message = string.Join("\n",teamStandings.Select(ts => $"{ts.Standing.Rank}: {ts.Team.FullName} ({ts.Team.Code}) | {ts.Standing.Points}P"));
        await ReplyAsync(message);

        timer.Stop();
        await Context.Channel.SendMessageAsync($"{timer.Elapsed.TotalMilliseconds}ms");
    }

    private async Task<string> GetTeamsForMatchAsync(Match match)
    {
        var homeTask = teamService.GetTeamAsync(match.HomeTeam.Id);
        var awayTask = teamService.GetTeamAsync(match.AwayTeam.Id);
        var teams = await Task.WhenAll(homeTask, awayTask);
        return $"{match.StartDateTime}: {teams[0].FullName} ({teams[0].Code}) vs {teams[1].FullName} ({teams[1].Code})";
    }
    private async Task<IEnumerable<Match>> GetNextMatchesAsync(IEnumerable<Match> matches)
    {
        var upcoming = matches.Where(m => m.StartDateTime.Date >= DateTime.UtcNow);
        if (!upcoming.Any())
        {
            return [];
        }
        var earliest = upcoming.Min(m => m.StartDateTime);
        return upcoming.Where(m => m.StartDateTime.Date == earliest.Date);
    }
}
