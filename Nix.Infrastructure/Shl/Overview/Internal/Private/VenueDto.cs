using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

internal record VenueDto([property: JsonPropertyName("name")] string Name);
