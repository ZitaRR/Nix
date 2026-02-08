using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Seasons.Private;

internal record SsgtFilterDto(
    [property: JsonPropertyName("season")] string SeasonId,
    [property: JsonPropertyName("gameType")] string GameTypeId
);