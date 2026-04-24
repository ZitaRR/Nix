using Nix.Domain.Core.Shl;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Orchestrator;

public interface IShlOrchestrator
{
    Task<Season> GetSeasonAsync();
}
