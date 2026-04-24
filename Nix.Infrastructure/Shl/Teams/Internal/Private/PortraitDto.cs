using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal record PortraitDto([property: JsonPropertyName("url")] string Url);
