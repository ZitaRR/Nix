using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Standings.Internal.Private;

internal record StandingsDto(
    [property: JsonPropertyName("stats")] ImmutableArray<StandingsTeamDto> Teams);
