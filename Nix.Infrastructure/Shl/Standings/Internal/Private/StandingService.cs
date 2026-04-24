using Nix.Infrastructure.Memory;
using Nix.Shared;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Standings.Internal.Private;

internal class StandingService(IHttpClientFactory httpClientFactory) : CacheBase<string, ImmutableArray<StandingsTeamDto>>, IStandingService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<ImmutableArray<StandingsTeamDto>> GetStandingsAsync(string ssgtId)
    {
        if (TryGet(ssgtId, out var value))
        {
            return value;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.STANDINGS_ENDPOINT + ssgtId);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<ImmutableArray<StandingsDto>>(content).FirstOrDefault() 
            ?? throw new OperationCanceledException("Unable to continue due to standings being empty");

        Set(ssgtId, dto.Teams);
        return dto.Teams;
    }
}
