using Nix.Infrastructure.Memory;
using Nix.Infrastructure.Shl.Seasons.Internal.Private;
using Nix.Shared;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Overview.Internal.Private;

internal class OverviewService(IHttpClientFactory httpClientFactory) : CacheBase<string, OverviewDto>, IOverviewService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<OverviewDto> GetGameInfoAsync(SeasonDto season, GameTypeDto type)
    {
        if (TryGet($"{season.Id}-{type.Id}", out var value))
        {
            return value;
        }

        string uri = string.Format(ShlConstants.GAME_INFO_ENDPOINT, season.Id, type.Id);
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<OverviewDto>(content);
        Set(nameof(Overview), dto);
        return dto;
    }
}
