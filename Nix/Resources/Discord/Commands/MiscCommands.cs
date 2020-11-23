using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System;

namespace Nix.Resources.Discord
{
    public class MiscCommands : ModuleBase<NixCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
            => await ReplyAsync("Pong!");

        [Command("info")]
        public async Task GetInfo(SocketGuildUser user = null)
        {
            if (user != null)
            {
                var nixUser = Context.GetNixUser;
                await ReplyAsync(nixUser.ToString());
                return;
            }

            await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                $"**Running On** ``{Context.NixClient.OS.VersionString}``\n" +
                $"**Uptime** ``{Context.NixClient.Watch.Elapsed:h\\:mm\\:ss}``");
        }
    }
}
