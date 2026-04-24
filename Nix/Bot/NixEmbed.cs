using Discord;
using Nix.Infrastructure.Bot;
using Nix.Shared;

namespace Nix.Bot;

internal static class NixEmbed
{
    internal static EmbedBuilder CreateNixBuilder(NixCommandContext context)
    {
        return new EmbedBuilder()
            .WithColor(new Color(255, 38, 176))
            .WithFooter(CreateNixFooter(context));
    }

    internal static EmbedFooterBuilder CreateNixFooter(NixCommandContext context) =>
        new EmbedFooterBuilder()
        .WithText($"{AppInfo.Version} {UnicodeConstants.LIGHTNING} {context.Latency()}ms");
}
