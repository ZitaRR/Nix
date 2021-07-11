using Discord.Commands;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using Discord.WebSocket;
using Nix.MVC;

namespace Nix.Resources.Discord
{
    public class MiscModule : ModuleBase<NixCommandContext>
    {
        private readonly CommandService commands;

        public MiscModule(CommandService commands)
        {
            this.commands = commands;
        }

        [Command("info")]
        public async Task GetInfo(SocketGuildUser user = null)
        {
            NixUser nixUser;
            if (user != null)
            {
                nixUser = await Context.GetNixUser(user.Id);
                await ReplyAsync(nixUser.ToString());
                return;
            }
            nixUser = await Context.GetNixUser();
            await ReplyAsync(nixUser.ToString());

            return;
            await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                $"**Uptime** ``{Context.NixClient.Watch.Elapsed:h\\:mm\\:ss}``\n" +
                $"**Version** ``{Utility.Version}``");
        }

        [Command("help")]
        public async Task Help()
        {
            var pages = new List<string>();
            foreach (var module in commands.Modules)
            {
                var content = $"**{module.Name.Replace("Module", "")}**\n\n";
                foreach (var command in module.Commands)
                {
                    var paramaters = string.Join(", ", command.Parameters);
                    paramaters = paramaters.Length <= 0 ? "" : $"<{paramaters}>";
                    content += $".{command.Name} {paramaters}\n";
                }
                pages.Add(content);
            }

            await Context.Reply.PaginatedMessageAsync(Context, pages);
        }
    }
}
