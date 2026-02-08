using Nix.Core.Shl.Overview.Private;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Teams.Private;

internal record TeamDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("icon")] string IconUrl,
    [property: JsonPropertyName("names")] NamesDto Names);
