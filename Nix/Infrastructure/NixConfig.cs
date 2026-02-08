using System;

namespace Nix.Infrastructure;

internal readonly record struct NixConfig
{
    internal readonly string DiscordToken { get; }

    public NixConfig(string discordToken)
    {
        if (string.IsNullOrEmpty(discordToken))
        {
            throw new ArgumentException($"{NixConstants.DISCORD_TOKEN} was not provided.", nameof(discordToken));
        }

        DiscordToken = discordToken;
    }
}
