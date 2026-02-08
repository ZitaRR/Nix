using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Standings.Private;

internal record InfoDto(
    [property: JsonPropertyName("uuid")] string TeamId);
