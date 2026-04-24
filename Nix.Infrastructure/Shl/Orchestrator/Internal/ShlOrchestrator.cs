using Nix.Domain.Core.Shl;
using Nix.Infrastructure.Memory;
using Nix.Infrastructure.Shl.Overview.Internal;
using Nix.Infrastructure.Shl.Seasons.Internal;
using Nix.Infrastructure.Shl.Standings.Internal;
using Nix.Infrastructure.Shl.Teams.Internal;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Orchestrator.Internal;

internal class ShlOrchestrator(
    IOverviewService overviewService,
    ISeasonService seasonService,
    IStandingService standingsService,
    ITeamService teamService,
    IPlayerService playerService) : CacheBase<string, Season>, IShlOrchestrator
{
    public async Task<Season> GetSeasonAsync()
    {
        if (TryGet(nameof(ShlOrchestrator), out var value))
        {
            return value;
        }

        var gameTypes = await seasonService.GetGameTypesAsync();
        var (activeSeason, ssgtId) = await seasonService.GetActiveSeasonAsync();
        var regularTask = overviewService.GetGameInfoAsync(activeSeason, gameTypes.First(gt => gt.Code.Equals("regular", StringComparison.OrdinalIgnoreCase)));
        var playoffsTask = overviewService.GetGameInfoAsync(activeSeason, gameTypes.First(gt => gt.Code.Equals("playoff", StringComparison.OrdinalIgnoreCase)));
        var relegationTask = overviewService.GetGameInfoAsync(activeSeason, gameTypes.First(gt => gt.Code.Equals("qualdown", StringComparison.OrdinalIgnoreCase)));
        var standingsTask = standingsService.GetStandingsAsync(ssgtId);

        await Task.WhenAll(regularTask, playoffsTask, relegationTask, standingsTask);
        var regular = await regularTask;
        var playoffs = await playoffsTask;
        var relegations = await relegationTask;
        var standings = await standingsTask;

        var tasks = standings.Select(async s =>
        {
            var teamId = s.Info.TeamId;

            var team = await teamService.GetTeamAsync(teamId);
            var players = await playerService.GetPlayersAsync(teamId);

            return (Team: team, Players: players);
        });

        var result = await Task.WhenAll(tasks);
        var builder = ImmutableArray.CreateBuilder<Team>();
        foreach (var (team, players) in result)
        {
            builder.Add(team.ToTeam(players, standings));
        }

        var teams = builder.ToImmutable();
        var season = new Season(
            [.. regular.Matches.Select(m => m.ToMatch(teams))],
            [.. playoffs.Matches.Select(m => m.ToMatch(teams))],
            [.. relegations.Matches.Select(m => m.ToMatch(teams))],
            teams);

        Set(nameof(ShlOrchestrator), season);
        return season;
    }
}
