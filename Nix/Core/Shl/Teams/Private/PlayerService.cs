using Nix.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Teams.Private;

internal class PlayerService(IHttpClientFactory httpClientFactory) : CacheBase<string, IEnumerable<Player>>, IPlayerService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<IEnumerable<Player>> GetPlayersAsync(string teamId)
    {
        if (TryGet(teamId, out var value))
        {
            return value;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.PLAYERS_ENDPOINT + teamId);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var containers = JsonSerializer.Deserialize<IEnumerable<PlayersContainerDto>>(content);

        var players = containers.SelectMany(c => c.Players
            .Where(p => p.Number > 0)
            .Select(p => Player.Create(p, c.PositionCode.ToPlayerPosition())));

        Set(teamId, players);
        return players;
    }
}
