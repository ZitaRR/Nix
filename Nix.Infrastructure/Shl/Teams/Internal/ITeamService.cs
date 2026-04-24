using Nix.Infrastructure.Shl.Teams.Internal.Private;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Teams.Internal;

internal interface ITeamService
{
    Task<TeamDto> GetTeamAsync(string teamId);
}
