using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Settings.Internal.Private;

internal record ConfigDto(
    [property: JsonPropertyName("logo")] string LogoUri);
