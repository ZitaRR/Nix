using System.Linq;
using System.Threading.Tasks;
using Nix.Core.Shl.Seasons;

namespace Nix.Core.Shl;

public class ShlContext(ISeasonService seasonsService)
{
    public async Task<Season> GetCurrentSeasonAsync() =>
        await seasonsService.GetActiveSeasonAsync();

    public async Task<GameType> GetRegularSeasonAsync()
    {
        var types = await seasonsService.GetGameTypesAsync();
        return types.First(t => t.State == GameState.Regular);
    }

    public async Task<GameType> GetPlayoffsAsync()
    {
        var types = await seasonsService.GetGameTypesAsync();
        return types.First(t => t.State == GameState.Playoffs);
    }

    public async Task<GameType> GetRelegationAsync()
    {
        var types = await seasonsService.GetGameTypesAsync();
        return types.First(t => t.State == GameState.Relegation);
    }
}