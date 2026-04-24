using Nix.Infrastructure.Shl.Standings.Internal.Private;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Standings.Internal;

internal interface IStandingService
{
    Task<ImmutableArray<StandingsTeamDto>> GetStandingsAsync(string ssgtId);
}
