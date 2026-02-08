using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Nix.Infrastructure;

namespace Nix.Core.Shl.Seasons.Private;

internal class SeasonService(IHttpClientFactory httpClientFactory) : CacheBase<string, SeasonService.Data>, ISeasonService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);
    
    public async Task<Season> GetActiveSeasonAsync()
    {
        if (TryGet(nameof(SeasonService), out var value))
        {
            return value.ActiveSeason;
        }

        var dto = await SendAsync();
        var data = Data.Create(dto);

        Set(nameof(SeasonService), data);
        return data.ActiveSeason;
    }
    public async Task<GameType> GetActiveGameTypeAsync()
    {
        if (TryGet(nameof(SeasonService), out var value))
        {
            return value.ActiveGameType;
        }

        var dto = await SendAsync();
        var data = Data.Create(dto);

        Set(nameof(SeasonService), data);
        return data.ActiveGameType;
    }

    public async Task<IEnumerable<GameType>> GetGameTypesAsync()
    {
        if (TryGet(nameof(SeasonService), out var value))
        {
            return value.GameTypes;
        }

        var dto = await SendAsync();
        var data = Data.Create(dto);

        Set(nameof(SeasonService), data);
        return data.GameTypes;
    }

    private async Task<SeasonsDto> SendAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.SEASONS_ENDPOINT);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SeasonsDto>(content);
    }

    internal record Data(
        Season ActiveSeason,
        GameType ActiveGameType,
        IEnumerable<Season> Seasons, 
        IEnumerable<GameType> GameTypes)
    {
        internal static Data Create(SeasonsDto dto)
        {
            var activeSeason = dto.Seasons.First(s => s.Id.Equals(dto.Filter.SeasonId, StringComparison.OrdinalIgnoreCase));
            var activeGameType = dto.GameTypes.First(gt => gt.Id.Equals(dto.Filter.GameTypeId, StringComparison.OrdinalIgnoreCase));
            
            return new(
                Season.Create(activeSeason),
                GameType.Create(activeGameType),
                dto.Seasons.Select(Season.Create), 
                dto.GameTypes.Select(GameType.Create));
        }
    }
}