using System;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

internal record MatchDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("ssgtUuid")] string SeasonId,
    [property: JsonPropertyName("state")] StateDto State,
    [property: JsonPropertyName("homeTeamInfo")] OverviewTeamDto HomeTeam,
    [property: JsonPropertyName("awayTeamInfo")] OverviewTeamDto AwayTeam,
    [property: JsonPropertyName("venueInfo")] VenueDto Venue,
    [property: JsonPropertyName("rawStartDateTime")] DateTime StartDateTime);
