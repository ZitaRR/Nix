using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Seasons.Private;

internal record SeasonsDto(
    [property: JsonPropertyName("ssgtUuid")] string SsgtId,
    [property: JsonPropertyName("defaultSsgtFilter")] SsgtFilterDto Filter,
    [property: JsonPropertyName("season")] IEnumerable<SeasonDto> Seasons,
    [property: JsonPropertyName("gameType")] IEnumerable<GameTypeDto> GameTypes);