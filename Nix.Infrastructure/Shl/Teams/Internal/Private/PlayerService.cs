using Nix.Domain.Core.Shl;
using Nix.Infrastructure.Memory;
using Nix.Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Teams.Internal.Private;

internal class PlayerService(IHttpClientFactory httpClientFactory) : CacheBase<string, PlayerService.Data>, IPlayerService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<ImmutableArray<(PlayerDto Player, Position Position)>> GetPlayersAsync(string teamId)
    {
        if (TryGet(teamId, out var value))
        {
            return value.Players;
        }

        var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.PLAYERS_ENDPOINT + teamId);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var containers = JsonSerializer.Deserialize<IEnumerable<PlayersContainerDto>>(content);

        var players = containers.SelectMany(c => c.Players
            .Where(p => p.Number > 0)
            .Select(p => (p, c.PositionCode)))
            .ToImmutableArray();
        var data = Data.Create(players);

        Set(teamId, data);
        return data.Players;
    }

    internal record Data(ImmutableArray<(PlayerDto, Position)> Players)
    {
        internal static Data Create(ImmutableArray<(PlayerDto Player, string PositionCode)> players) =>
            new([.. players.Select(e => (e.Player, e.PositionCode.ToPlayerPosition()))]);
    }
}
