using Nix.Core.Shl.Seasons;
using Nix.Infrastructure;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Overview.Private;

internal class OverviewService(IHttpClientFactory httpClientFactory) : CacheBase<string, Overview>, IOverviewService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<Overview> GetGameInfoAsync(Season season, GameType type)
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
        var overview = Overview.Create(dto);
        Set(nameof(Overview), overview);
        return overview;
    }
}
