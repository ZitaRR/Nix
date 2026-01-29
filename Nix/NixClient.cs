using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nix;

internal class NixClient(
    ILogger<NixClient> logger, 
    NixConfig config,
    DiscordSocketClient client,
    CommandHandler commandHandler) 
    : IHostedService, IAsyncDisposable
{
    public async Task StartAsync(CancellationToken ct)
    {
        logger.LogInformation("Establishing connection with Discord...");

        client.Ready += OnReady;

        await commandHandler.InitializeAsync();
        await client.LoginAsync(Discord.TokenType.Bot, config.DiscordToken);
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
