using Discord.Addons.Interactive;
using Discord.Commands;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class MiscCommands : NixContext
    {
        [Command("ping")]
        public async Task Ping()
            => await ReplyAsync("Pong!");
    }
}
