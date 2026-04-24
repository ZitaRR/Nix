using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal record SeasonDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("names")] ImmutableArray<NamesDto> Names
);