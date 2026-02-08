using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix.Core.Discord.Private;

internal class CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
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
