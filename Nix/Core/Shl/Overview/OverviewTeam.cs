using Nix.Core.Shl.Overview.Private;

namespace Nix.Core.Shl.Overview;

public record OverviewTeam(string Id)
{
    internal static OverviewTeam Create(TeamDto dto) =>
        new(dto.Id);
}
