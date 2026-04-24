using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

internal record OverviewDto([property: JsonPropertyName("gameInfo")] ImmutableArray<MatchDto> Matches);
