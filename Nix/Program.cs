using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nix.Core.Discord.Private;
using Nix.Core.Shl;
using Nix.Core.Shl.Overview;
using Nix.Core.Shl.Overview.Private;
using Nix.Core.Shl.Seasons;
using Nix.Core.Shl.Seasons.Private;
using Nix.Core.Shl.Standings;
using Nix.Core.Shl.Standings.Private;
using Nix.Core.Shl.Teams;
using Nix.Core.Shl.Teams.Private;
using Nix.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Nix;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .CreateNixConfig();

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHostedService<NixClient>();
                services.AddSingleton(typeof(NixConfig), config);

                services.AddSingleton<ShlContext>();
                services.AddSingleton<IOverviewService, OverviewService>();
                services.AddSingleton<IPlayerService, PlayerService>();
                services.AddSingleton<IStandingsService, StandingsService>();
                services.AddSingleton<ITeamService, TeamService>();
                services.AddSingleton<ISeasonService, SeasonService>();

                services.AddHttpClient(ShlConstants.CLIENT, client => client.BaseAddress = new Uri(ShlConstants.BASE_URL));

                services.AddSingleton<DiscordSocketClient>(_ => new(new DiscordSocketConfig
                {
                    GatewayIntents = Discord.GatewayIntents.All
                }));
                services.AddSingleton<CommandService>();
                services.AddSingleton<CommandHandler>();
            });

        await builder.RunConsoleAsync();
    }
}
