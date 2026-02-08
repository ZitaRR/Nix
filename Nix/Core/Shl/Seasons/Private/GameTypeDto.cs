using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Seasons.Private;

internal record GameTypeDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("code")] string Code
);