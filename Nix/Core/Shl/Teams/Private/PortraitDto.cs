using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Teams.Private;

public record PortraitDto([property: JsonPropertyName("url")] string Url);
