using System.Threading.Tasks;

namespace Nix.Core.Shl.Teams;

public interface ITeamService
{
    Task<Team> GetTeamAsync(string teamId);
}
