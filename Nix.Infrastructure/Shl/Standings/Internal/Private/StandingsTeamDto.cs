using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Standings.Internal.Private;

internal record StandingsTeamDto(
    [property: JsonPropertyName("Rank")] int Rank,
    [property: JsonPropertyName("Points")] int Points,
    [property: JsonPropertyName("GP")] int GamesPlayed,
    [property: JsonPropertyName("info")] InfoDto Info);
