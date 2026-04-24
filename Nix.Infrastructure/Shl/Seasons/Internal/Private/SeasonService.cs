using Nix.Infrastructure.Memory;
using Nix.Shared;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Seasons.Internal.Private;

internal class SeasonService(IHttpClientFactory httpClientFactory) : CacheBase<string, SeasonService.Data>, ISeasonService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);
    
    public async Task<(SeasonDto Season, string SsgtId)> GetActiveSeasonAsync()
    {
        if (TryGet(nameof(SeasonService), out var value))
        {
            return (value.ActiveSeason, value.SsgtId);
        }

        var dto = await SendAsync();
        var data = Data.Create(dto);

        Set(nameof(SeasonService), data);
        return (data.ActiveSeason, data.SsgtId);
    }
    public async Task<GameTypeDto> GetActiveGameTypeAsync()
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

    public async Task<ImmutableArray<GameTypeDto>> GetGameTypesAsync()
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
        string SsgtId,
        SeasonDto ActiveSeason,
        GameTypeDto ActiveGameType,
        ImmutableArray<SeasonDto> Seasons, 
        ImmutableArray<GameTypeDto> GameTypes)
    {
        internal static Data Create(SeasonsDto dto)
        {
            var activeSeason = dto.Seasons.First(s => s.Id.Equals(dto.Filter.SeasonId, StringComparison.OrdinalIgnoreCase));
            var activeGameType = dto.GameTypes.First(gt => gt.Id.Equals(dto.Filter.GameTypeId, StringComparison.OrdinalIgnoreCase));
            
            return new(
                dto.SsgtId,
                activeSeason,
                activeGameType,
                [.. dto.Seasons],
                [.. dto.GameTypes]);
        }
    }
}