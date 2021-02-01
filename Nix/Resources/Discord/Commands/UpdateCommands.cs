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
            if (!UpdateAvailable())
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
            if (!UpdateAvailable())
            {
                await Context.Reply.ErrorAsync(Context.Channel as ITextChannel,
                    "There's nothing new, please use the ``check`` command next time!");
                return;
            }

            await Context.Reply.MessageAsync(Context.Channel as ITextChannel,
                "This might take a few moments.\n" +
                "Updating...");

            Context.Script.RunScript("update.ps1", out string result, false);
            await Context.Reply.MessageAsync(Context.Channel as ITextChannel, result);

            Process.Start(new ProcessStartInfo("dotnet", Assembly.GetEntryAssembly().Location));
            Environment.Exit(0);
        }

        [Command("restart")]
        public async Task Restart()
        {
            await Context.Reply.MessageAsync(Context.Channel as ITextChannel, "Restarting...");
            Process.Start(new ProcessStartInfo("dotnet", Assembly.GetEntryAssembly().Location));
            Environment.Exit(0);
        }

        private bool UpdateAvailable()
        {
            Context.Script.RunScript("check_update.ps1", out string result, false);

            return result switch
            {
                "Up to date" => false,
                "Update available" => true,
                _ => false,
            };
        }
    }
}
