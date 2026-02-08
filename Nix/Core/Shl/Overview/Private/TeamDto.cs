using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Overview.Private;

internal record TeamDto([property: JsonPropertyName("uuid")] string Id);
