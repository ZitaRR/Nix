using Nix.Infrastructure;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nix.Core.Shl.Teams.Private;

internal class TeamService(
    IHttpClientFactory httpClientFactory,
    IPlayerService playerService) : CacheBase<string, Team>, ITeamService
{
    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<Team> GetTeamAsync(string teamId)
    {
        if (TryGet(teamId, out var value))
        {
            return value;
        }

        var teamTask = GetTeamAsync();
        var playersTask = playerService.GetPlayersAsync(teamId);

        await Task.WhenAll(teamTask, playersTask);
        var dto = await teamTask;
        var players = await playersTask;

        var team = Team.Create(dto, players);
        Set(teamId, team);
        return team;

        async Task<TeamDto> GetTeamAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ShlConstants.TEAMS_ENDPOINT + teamId);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<TeamDto>(content);
            return dto;
        }
    }
}
