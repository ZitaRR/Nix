using System.Threading.Tasks;

namespace Nix.Core.Shl.Overview;

public interface IOverviewService 
{
    Task<Overview> GetGameInfoAsync(); 
}
