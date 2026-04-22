using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Teams.Private;

public record PlayersContainerDto(
    [property: JsonPropertyName("positionCode")] string PositionCode,
    [property: JsonPropertyName("players")] IEnumerable<PlayerDto> Players);
