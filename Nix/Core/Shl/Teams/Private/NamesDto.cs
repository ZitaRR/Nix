using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Teams.Private;

internal record NamesDto(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("long")] string Long,
    [property: JsonPropertyName("full")] string Full);
