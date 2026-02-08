using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Standings.Private;

internal record TeamDto(
    [property: JsonPropertyName("Rank")] int Rank,
    [property: JsonPropertyName("Points")] int Points,
    [property: JsonPropertyName("GP")] int GamesPlayed,
    [property: JsonPropertyName("info")] InfoDto Info);
