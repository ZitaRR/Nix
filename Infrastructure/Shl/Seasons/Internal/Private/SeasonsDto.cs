using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal record SeasonsDto(
    [property: JsonPropertyName("ssgtUuid")] string SsgtId,
    [property: JsonPropertyName("defaultSsgtFilter")] SsgtFilterDto Filter,
    [property: JsonPropertyName("season")] ImmutableArray<SeasonDto> Seasons,
    [property: JsonPropertyName("gameType")] ImmutableArray<GameTypeDto> GameTypes);