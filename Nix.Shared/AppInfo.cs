namespace Nix.Shared;

public readonly struct AppInfo
{
    public static string Version { get; private set; }

    public AppInfo(string version)
    {
        Version = version;
    }
}
