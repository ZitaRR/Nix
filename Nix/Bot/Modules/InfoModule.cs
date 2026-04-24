using Discord;
using Discord.Commands;
using Nix.Infrastructure.Bot;
using Nix.Shared;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nix.Bot.Modules;

public class InfoModule : ModuleBase<NixCommandContext>
{
    private const string GIT_COMMIT = "https://github.com/ZitaRR/Nix/commit/";

    [Command("info")]
    [Summary($"Provides info about {NixConstants.NIX}.")]
    public async Task InfoAsync()
    {
        var embed = NixEmbed.CreateNixBuilder()
            .WithAuthor(new EmbedAuthorBuilder().WithName(NixConstants.NIX).WithIconUrl("attachment://logo.png"))
            .WithDescription(
            $"**Uptime** {(int)Math.Floor(AppInfo.Uptime.TotalHours)}h {AppInfo.Uptime.Minutes}m\n" +
            $"**Latency** {Context.Latency()}ms\n" +
            $"### {UnicodeConstants.LAPTOP} __System__\n" +
            $"**OS** {RuntimeInformation.OSDescription}\n" +
            $"**Runtime** {RuntimeInformation.FrameworkDescription}\n" +
            $"**Memory** {GC.GetTotalMemory(false) / 1024 / 1024}mb\n" +
            $"### {UnicodeConstants.BOX} __Build__\n" +
            $"**Version** {AppInfo.Version}\n" +
            $"**Commit** [{AppInfo.Hash}]({GIT_COMMIT}{AppInfo.Hash})")
            .Build();

        await Context.Channel.SendFileAsync(NixConstants.LogoPath, embed: embed);
    }
}
