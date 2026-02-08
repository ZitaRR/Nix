using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Overview.Private;

internal record VenueDto([property: JsonPropertyName("name")] string Name);
