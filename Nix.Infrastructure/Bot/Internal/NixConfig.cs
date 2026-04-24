using Nix.Shared;
using System;

namespace Nix.Infrastructure.Bot.Internal;

internal readonly record struct NixConfig
{
    internal static string DiscordToken { get; private set; }

    public NixConfig(string discordToken)
    {
        if (string.IsNullOrEmpty(discordToken))
        {
            throw new ArgumentException($"{NixConstants.DISCORD_TOKEN} was not provided.", nameof(discordToken));
        }

        DiscordToken = discordToken;
    }
}
