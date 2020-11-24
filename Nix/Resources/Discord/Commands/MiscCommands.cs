using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System;
using System.Globalization;

namespace Nix.Resources.Discord
{
    public class MiscCommands : ModuleBase<NixCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.User.SendMessageAsync("Pong!");
        }

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

        [Command("test")]
        public async Task TestAsync([Remainder] string input)
        {
            input = $"{DateTime.UtcNow.Year}/{input}";
            if (DateTime.TryParse(input, out var date))
            {
                await Context.Channel.SendMessageAsync(date.ToString("yyyy-MM-dd, HH:mm UTC"));
                return;
            }
            await Context.Channel.SendMessageAsync("Date/Time was invalid");
        }
    }
}
