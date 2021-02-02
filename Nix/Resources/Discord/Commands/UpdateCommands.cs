using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix.Resources.Discord.Commands
{
    public class UpdateCommands : ModuleBase<NixCommandContext>
    {
        [Command("check")]
        public async Task CheckUpdate()
        {
            if (!(await UpdateAvailable()))
            {
                await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                        "There's nothing new, I'm up to date!");
                return;
            }

            await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                        "There's an update available!");
        }

        [Command("update")]
        public async Task Update()
        {
            if (!(await UpdateAvailable()))
            {
                await Context.Reply.ErrorAsync(Context.Channel as ITextChannel,
                    "There's nothing new, please use the ``check`` command next time!");
                return;
            }

            await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                "This might take a few moments.\n" +
                "Updating...");

            var result = await Context.Script.RunScript("update.ps1");
            await Context.Reply.MessageAsync(Context.Channel as ITextChannel, result);

            await Context.Script.RunScript("run_nix.ps1", false);
            Environment.Exit(0);
        }

        [Command("restart")]
        public async Task Restart()
        {
            await Context.Reply.MessageAsync(Context.Channel as ITextChannel, "Restarting...");
            await Context.Script.RunScript("run_nix.ps1", false);
            Environment.Exit(0);
        }

        private async Task<bool> UpdateAvailable()
        {
            var result = await Context.Script.RunScript("check_update.ps1");

            return result switch
            {
                "Up to date" => false,
                "Update available" => true,
                _ => false,
            };
        }
    }
}
