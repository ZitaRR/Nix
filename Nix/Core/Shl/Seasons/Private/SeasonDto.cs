using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Seasons.Private;

internal record SeasonDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("names")] NamesDto[] Names
);