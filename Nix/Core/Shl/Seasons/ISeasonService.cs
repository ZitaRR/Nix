using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Seasons;

public interface ISeasonService
{
    Task<Season> GetActiveSeasonAsync();

    Task<GameType> GetActiveGameTypeAsync();

    Task<IEnumerable<GameType>> GetGameTypesAsync();
}