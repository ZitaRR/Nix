using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Teams;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetPlayersAsync(string teamId);
}
