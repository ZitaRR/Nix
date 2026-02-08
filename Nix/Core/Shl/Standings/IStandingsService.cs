using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Standings;

public interface IStandingsService
{
    Task<IEnumerable<StandingsTeam>> GetStandingsAsync(string seasonId);
}
