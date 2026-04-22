using System.Threading.Tasks;
using Nix.Core.Shl.Seasons;

namespace Nix.Core.Shl.Overview;

public interface IOverviewService 
{
    Task<Overview> GetGameInfoAsync(Season season, GameType type); 
}
