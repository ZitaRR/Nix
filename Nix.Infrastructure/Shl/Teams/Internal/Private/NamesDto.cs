using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal record NamesDto(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("long")] string Long,
    [property: JsonPropertyName("full")] string Full);
