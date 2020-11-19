using Discord.Addons.Interactive;
using Discord.Commands;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixContext : ModuleBase
    {
        public async Task ReplyAsync(string message)
            => await Context.Channel.SendMessageAsync(message);
    }
}
