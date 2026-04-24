using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal record PlayersContainerDto(
    [property: JsonPropertyName("positionCode")] string PositionCode,
    [property: JsonPropertyName("players")] ImmutableArray<PlayerDto> Players);
