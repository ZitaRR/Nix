using Discord;
using Discord.Commands;
using Nix.Domain.Core.Shl;
using Nix.Infrastructure.Bot;
using Nix.Infrastructure.Shl.Orchestrator;
using Nix.Shared;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nix.Bot.Modules;

[Group("shl")]
public class ShlModule(IShlOrchestrator shlOrchestrator) : ModuleBase<NixCommandContext>
{

    [Command("team")]
    [Summary("Displays the specified team")]
    [Alias("t")]
    public async Task Test([Remainder][Summary("The team to display")] string input)
    {
        var season = await shlOrchestrator.GetSeasonAsync();

        var team = season.Teams.FirstOrDefault(t => t.Name.Contains(input, StringComparison.OrdinalIgnoreCase));
        if (team is null)
        {
            await ReplyAsync($"{UnicodeConstants.X} No team found");
            return;
        }
        
        var forwards = team.Players.Where(p => p.Position is Position.Forward);
        var defensemen = team.Players.Where(p => p.Position is Position.Defense);
        var goalies = team.Players.Where(p => p.Position is Position.Goalie);

        var embed = NixEmbed.CreateNixBuilder()
            .WithTitle(team.Name)
            .WithThumbnailUrl($"attachment://team.png")
            .WithFields(
            new EmbedFieldBuilder().WithIsInline(false).WithName($"{UnicodeConstants.GOAL_NET} Goalies").WithValue(goalies.ListPlayers()),
            new EmbedFieldBuilder().WithIsInline(true).WithName($"{UnicodeConstants.HOCKEY_CLUB} Forwards").WithValue(forwards.ListPlayers()),
            new EmbedFieldBuilder().WithIsInline(true).WithName($"{UnicodeConstants.SHIELD} Defensemen").WithValue(defensemen.ListPlayers()))
            .WithNixFooter(Context)
            .Build();

        await Context.Channel.SendFileAsync(new MemoryStream(team.IconBytes), "team.png", embed: embed);
    }

    [Command("upcoming")]
    [Summary("Lists the upcoming games.")]
    [Alias("up")]
    public async Task ListShlTeamsAsync()
    {
        var season = await shlOrchestrator.GetSeasonAsync();
        var matches = GetNextMatches(season.RegularMatches);

        if (!matches.Any())
        {
            matches = GetNextMatches(season.RelegationMatches);
        }

        if (!matches.Any())
        {
            matches = GetNextMatches(season.PlayoffMatches);
        }

        if (!matches.Any())
        {
            await ReplyAsync("Season has ended!");
            return;
        }

        var fields = matches.Select(m => new EmbedFieldBuilder().WithName($"{m.HomeTeam.Name} ({m.HomeTeam.Code}) vs {m.AwayTeam.Name} ({m.AwayTeam.Code})").WithValue(m.StartDateTime));
        var embed = NixEmbed.CreateNixBuilder()
            .WithFields(fields)
            .WithNixFooter(Context)
            .Build();

        await Context.Channel.SendMessageAsync(embed: embed);
    }

    [Command("standings")]
    [Summary("Lists the team standings.")]
    [Alias("score")]
    public async Task StandingsAsync()
    {
        var season = await shlOrchestrator.GetSeasonAsync();

        var standings = string.Join("\n", season.Teams.Select(t => $"**{t.Standing.Rank}** {t.Name} ({t.Code}) | {t.Standing.Points}"));
        var embed = NixEmbed.CreateNixBuilder()
            .WithDescription(standings)
            .WithNixFooter(Context)
            .Build();

        await Context.Channel.SendMessageAsync(embed: embed);
    }

    private ImmutableArray<Match> GetNextMatches(ImmutableArray<Match> matches)
    {
        var upcoming = matches.Where(m => m.StartDateTime.Date >= DateTime.UtcNow);
        if (!upcoming.Any())
        {
            return [];
        }
        var earliest = upcoming.Min(m => m.StartDateTime);
        return [.. upcoming.Where(m => m.StartDateTime.Date == earliest.Date)];
    }
}
