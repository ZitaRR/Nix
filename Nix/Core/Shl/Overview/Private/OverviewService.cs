using Nix.Infrastructure;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Overview.Private;

internal class OverviewService(IHttpClientFactory httpClientFactory) : CacheBase<string, Overview>, IOverviewService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<Overview> GetGameInfoAsync()
    {
        if (TryGet(nameof(Overview), out var value))
        {
            return value;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.GAME_INFO_ENDPOINT);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<OverviewDto>(content);
        var overview = Overview.Create(dto);
        Set(nameof(Overview), overview);
        return overview;
    }
}
