using Nix.Infrastructure.Shl.Seasons.Internal.Private;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Seasons.Internal;

internal interface ISeasonService
{
    Task<(SeasonDto Season, string SsgtId)> GetActiveSeasonAsync();

    Task<GameTypeDto> GetActiveGameTypeAsync();

    Task<ImmutableArray<GameTypeDto>> GetGameTypesAsync();
}