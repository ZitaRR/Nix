using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Standings.Private;

internal record StandingsDto(
    [property: JsonPropertyName("stats")] IEnumerable<TeamDto> Teams);
