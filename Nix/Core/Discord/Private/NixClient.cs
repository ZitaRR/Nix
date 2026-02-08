using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Nix.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Nix.Core.Discord.Private;

internal class NixClient(
    ILogger<NixClient> logger, 
    NixConfig config,
    DiscordSocketClient client,
    CommandHandler commandHandler) 
    : INixClient
{
    public async Task StartAsync(CancellationToken ct)
    {
        logger.LogInformation("Establishing connection with Discord...");

        client.Ready += OnReady;

        await commandHandler.InitializeAsync();
        await client.LoginAsync(TokenType.Bot, config.DiscordToken);
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
