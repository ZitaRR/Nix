using System;

namespace Nix.Shared;

public readonly struct AppInfo
{
    public static bool IsDebug { get; private set; }
    public static string Version { get; private set; }
    public static string Hash { get; private set; }
    public static string CompleteVersion => $"{Version}-{Hash}";
    public static TimeSpan Uptime => DateTime.UtcNow - startTime;

    private static readonly DateTime startTime = DateTime.UtcNow;

    public AppInfo(bool debug, string version, string hash)
    {
        IsDebug = debug;
        Version = $"v{version}";
        Hash = hash;
    }
}
