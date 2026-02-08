using Nix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Standings.Private;

internal class StandingsService(IHttpClientFactory httpClientFactory) : CacheBase<string, IEnumerable<StandingsTeam>>, IStandingsService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<IEnumerable<StandingsTeam>> GetStandingsAsync(string seasonId)
    {
        if (TryGet(seasonId, out var value))
        {
            return value;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.STANDINGS_ENDPOINT + seasonId);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<IEnumerable<StandingsDto>>(content).FirstOrDefault();
        if (dto is null)
        {
            throw new OperationCanceledException("Unable to continue due to standings being empty");
        }

        var teams = dto.Teams.Select(StandingsTeam.Create);
        Set(seasonId, teams);
        return teams;
    }
}
