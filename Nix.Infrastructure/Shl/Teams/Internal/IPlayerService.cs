using Nix.Domain.Core.Shl;
using Nix.Infrastructure.Shl.Teams.Internal.Private;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Teams.Internal;

internal interface IPlayerService
{
    Task<ImmutableArray<(PlayerDto Player, Position Position)>> GetPlayersAsync(string teamId);
}
