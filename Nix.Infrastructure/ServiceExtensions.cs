using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nix.Infrastructure.Bot;
using Nix.Infrastructure.Bot.Internal;
using Nix.Infrastructure.Shl.Orchestrator;
using Nix.Infrastructure.Shl.Orchestrator.Internal;
using Nix.Infrastructure.Shl.Overview.Internal;
using Nix.Infrastructure.Shl.Overview.Internal.Private;
using Nix.Infrastructure.Shl.Seasons.Internal;
using Nix.Infrastructure.Shl.Seasons.Internal.Private;
using Nix.Infrastructure.Shl.Standings.Internal;
using Nix.Infrastructure.Shl.Standings.Internal.Private;
using Nix.Infrastructure.Shl.Teams.Internal;
using Nix.Infrastructure.Shl.Teams.Internal.Private;
using Nix.Shared;
using System;

namespace Nix.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationRoot root)
    {
        _ = new NixConfig(root[NixConstants.DISCORD_TOKEN]);

        services.AddSingleton<INixClient, NixClient>();
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<DiscordSocketClient>(_ => new(new DiscordSocketConfig
        {
            GatewayIntents = Discord.GatewayIntents.All
        }));

        services.AddShlIntegration();

        return services;
    }

    private static IServiceCollection AddShlIntegration(this IServiceCollection services)
    {
        services.AddHttpClient(ShlConstants.CLIENT, client => client.BaseAddress = new Uri(ShlConstants.BASE_URL));
        services.AddHttpClient(ShlConstants.LOGO_CLIENT, client => client.BaseAddress = new Uri(ShlConstants.LOGO_URL));
        return services
            .AddSingleton<IShlOrchestrator, ShlOrchestrator>()
            .AddSingleton<IOverviewService, OverviewService>()
            .AddSingleton<ISeasonService, SeasonService>()
            .AddSingleton<IStandingService, StandingService>()
            .AddSingleton<ITeamService, TeamService>()
            .AddSingleton<IPlayerService, PlayerService>();
    }
}
