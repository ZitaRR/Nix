using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal record SsgtFilterDto(
    [property: JsonPropertyName("season")] string SeasonId,
    [property: JsonPropertyName("gameType")] string GameTypeId
);