using Discord;
using Nix.Infrastructure.Bot;
using Nix.Shared;

namespace Nix.Bot;

internal static class NixEmbed
{
    internal static EmbedBuilder CreateNixBuilder()
    {
        return new EmbedBuilder()
            .WithColor(new Color(255, 38, 176));
    }

    internal static EmbedBuilder WithNixFooter(this EmbedBuilder builder, NixCommandContext context) =>
        builder.WithFooter(
            new EmbedFooterBuilder()
            .WithText($"{AppInfo.CompleteVersion} {UnicodeConstants.LIGHTNING} {context.Latency()}ms"));
}
