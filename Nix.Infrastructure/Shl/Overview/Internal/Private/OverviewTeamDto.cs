using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

internal record OverviewTeamDto(
    [property: JsonPropertyName("uuid")] string Id);
