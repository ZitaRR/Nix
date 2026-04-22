using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nix.Core.Shl.Overview.Private;

internal record OverviewDto([property: JsonPropertyName("gameInfo")] IEnumerable<MatchDto> Matches);
