using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal record TeamDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("icon")] string IconUrl,
    [property: JsonPropertyName("names")] NamesDto Names);
