using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal record GameTypeDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("code")] string Code
);