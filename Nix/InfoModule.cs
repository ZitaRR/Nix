using Discord.Commands;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nix;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("info")]
    [Summary($"Provides info about {NixConstants.NIX}.")]
    public async Task InfoAsync()
    {
        await ReplyAsync(
            $"..::** INFO **::..\n" +
            $"Machine:              {Environment.MachineName}\n" +
            $"OS Version:           {Environment.OSVersion}\n" +
            $"OS Description:       {RuntimeInformation.OSDescription}\n" +
            $"OS Architecture:      {RuntimeInformation.OSArchitecture}\n" +
            $"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
    }
}
