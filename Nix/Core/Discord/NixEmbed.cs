using Discord;
using Nix.Infrastructure;
using System;

namespace Nix.Core.Discord;

internal static class NixEmbed
{
    internal static EmbedBuilder CreateNixBuilder()
    {
        return new EmbedBuilder().WithFooter(CreateNixFooter());
    }

    internal static EmbedFooterBuilder CreateNixFooter() =>
        new EmbedFooterBuilder()
        .WithIconUrl($"attachment://{NixConstants.LogoPath}")
        .WithText($"{DateTime.UtcNow} UTC");
}
