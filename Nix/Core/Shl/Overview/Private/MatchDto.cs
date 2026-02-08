using System;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Overview.Private;

internal record MatchDto(
    [property: JsonPropertyName("uuid")] string Id,
    [property: JsonPropertyName("ssgtUuid")] string SeasonId,
    [property: JsonPropertyName("state")] StateDto State,
    [property: JsonPropertyName("homeTeamInfo")] TeamDto HomeTeam,
    [property: JsonPropertyName("awayTeamInfo")] TeamDto AwayTeam,
    [property: JsonPropertyName("venueInfo")] VenueDto Venue,
    [property: JsonPropertyName("rawStartDateTime")] DateTime StartDateTime);
