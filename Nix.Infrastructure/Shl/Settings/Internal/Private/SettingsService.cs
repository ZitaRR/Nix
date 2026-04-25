using Nix.Shared;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Settings.Internal.Private;

internal class SettingsService(IHttpClientFactory httpClientFactory) : ISettingsService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<SettingsDto> GetConfigAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.SETTINGS_ENDPOINT);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SettingsDto>(content);
    }
}
