using System;
using System.IO;

namespace Nix.Infrastructure;

internal static class NixConstants
{
    internal const string NIX = "Nix";
    internal const string DISCORD_TOKEN = nameof(DISCORD_TOKEN);

    internal static readonly string LogoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
}
