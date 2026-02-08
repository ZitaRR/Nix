using Nix.Core.Shl.Standings.Private;

namespace Nix.Core.Shl.Standings;

public record StandingsTeam(
    string Id,
    int Rank,
    int GamesPlayed,
    int Points)
{
    internal static StandingsTeam Create(TeamDto dto) =>
        new(
            dto.Info.TeamId,
            dto.Rank,
            dto.GamesPlayed,
            dto.Points);
}
