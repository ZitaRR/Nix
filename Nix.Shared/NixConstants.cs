using System;
using System.IO;

namespace Nix.Shared;

public static class NixConstants
{
    public const string NIX = "Nix";
    public const string DISCORD_TOKEN = nameof(DISCORD_TOKEN);

    public static readonly string LogoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
}
