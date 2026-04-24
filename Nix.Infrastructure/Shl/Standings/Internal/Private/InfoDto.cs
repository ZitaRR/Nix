using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Standings.Internal.Private;

internal record InfoDto(
    [property: JsonPropertyName("uuid")] string TeamId);
