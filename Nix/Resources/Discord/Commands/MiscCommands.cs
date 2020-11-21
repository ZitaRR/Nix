using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class MiscCommands : ModuleBase<NixCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
            => await ReplyAsync("Pong!");

        [Command("info")]
        public async Task GetInfo(SocketGuildUser user)
        {
            var nixUser = Context.GetNixUser;
            await ReplyAsync(nixUser.ToString());
        }
    }
}
