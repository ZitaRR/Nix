using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Nix.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Bot.Internal;

internal class NixClient(
    ILogger<NixClient> logger,
    DiscordSocketClient client,
    CommandHandler commandHandler) 
    : INixClient
{
    public async Task StartAsync(CancellationToken ct)
    {
        logger.LogInformation("Nix ({Version}) started.", AppInfo.Version);
        logger.LogInformation("Establishing connection with Discord...");

        client.Ready += OnReady;

        await commandHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, NixConfig.DiscordToken);
        await client.SetCustomStatusAsync(AppInfo.Version);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await client.LogoutAsync();
        await client.StopAsync();
    }

    private Task OnReady()
    {
        logger.LogInformation("Connection with Discord established.");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await client.DisposeAsync();
    }
}
