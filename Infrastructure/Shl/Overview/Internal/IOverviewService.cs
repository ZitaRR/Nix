using System.Threading.Tasks;
using Nix.Infrastructure.Shl.Overview.Internal.Private;
using Nix.Infrastructure.Shl.Seasons.Internal.Private;

namespace Nix.Infrastructure.Shl.Overview.Internal;

internal interface IOverviewService 
{
    Task<OverviewDto> GetGameInfoAsync(SeasonDto season, GameTypeDto type); 
}
