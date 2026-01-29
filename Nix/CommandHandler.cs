using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix;

internal class CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, ILogger<CommandHandler> logger)
{
    public async Task InitializeAsync()
    {
        client.MessageReceived += HandleMessageAsync;
        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    private async Task HandleMessageAsync(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage message || socketMessage.Author.IsBot)
        {
            return;
        }

        var argPos = 0;
        if (!message.HasCharPrefix('!', ref argPos) &&
            !message.HasMentionPrefix(client.CurrentUser, ref argPos))
        {
            return;
        }

        var context = new SocketCommandContext(client, message);
        await commands.ExecuteAsync(context, argPos, services);
    }
}
