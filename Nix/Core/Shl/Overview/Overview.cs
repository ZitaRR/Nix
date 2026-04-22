using Nix.Core.Shl.Overview.Private;
using System.Collections.Generic;
using System.Linq;

namespace Nix.Core.Shl.Overview;

public record Overview(
    string Id,
    IEnumerable<Match> Matches)
{
    internal static Overview Create(OverviewDto dto) =>
        new(dto.Matches.First().SeasonId, dto.Matches.Select(Match.Create));
}
