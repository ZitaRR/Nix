using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace Nix.Core;

internal static class Machine
{
    private const string UNKNOWN_CPU = "Unknown CPU";

    internal static string GetCpu()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsCpuName();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxCpuName();
        }

        return UNKNOWN_CPU;
    }

    internal static double GetUsedRamInGb()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ToGb(GetUsedRamBytesWmi());
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return ToGb(GetUsedRamBytesLinux());
        }

        return 0;
    }

    internal static double GetTotalRamInGb()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ToGb(GetTotalRamBytesWmi());
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return ToGb(GetTotalRamBytesLinux());
        }

        return 0;
    }

    private static double ToGb(ulong bytes)
    {
        double gb = bytes / 1024d / 1024d / 1024d;
        return gb;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private static string GetWindowsCpuName()
    {
        using var searcher = new ManagementObjectSearcher(
            "select Name from Win32_Processor");

        foreach (var item in searcher.Get())
        {
            return item["Name"]?.ToString()?.Trim();
        }

        return UNKNOWN_CPU;
    }

    private static string GetLinuxCpuName()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "lscpu",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var vendor = string.Empty;
        var model = string.Empty;
        using var process = Process.Start(psi);
        foreach (var line in process.StandardOutput.ReadToEnd().Split('\n'))
        {
            if (line.TrimStart().StartsWith("Vendor ID:"))
            {
                vendor = line.Split(':', 2)[1].Trim();
            }

            if (line.TrimStart().StartsWith("Model name:"))
            {
                model = line.Split(':', 2)[1].Trim();
            }
        }

        if (string.IsNullOrEmpty(vendor) &&
            string.IsNullOrEmpty(model))
        {
            return UNKNOWN_CPU;
        }

        return $"{vendor} {model}";
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private static ulong GetUsedRamBytesWmi()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");

        foreach (ManagementObject obj in searcher.Get())
        {
            ulong total = (ulong)obj["TotalVisibleMemorySize"] * 1024;
            ulong free = (ulong)obj["FreePhysicalMemory"] * 1024;
            return total - free;
        }

        return 0;
    }

    private static ulong GetUsedRamBytesLinux()
    {
        ulong total = 0;
        ulong available = 0;

        foreach (var line in File.ReadLines("/proc/meminfo"))
        {
            if (line.StartsWith("MemTotal:"))
                total = ParseKb(line);

            else if (line.StartsWith("MemAvailable:"))
                available = ParseKb(line);
        }

        return (total - available) * 1024;

        static ulong ParseKb(string line)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return ulong.Parse(parts[1]); // value in kB
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    private static ulong GetTotalRamBytesWmi()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");

        foreach (ManagementObject obj in searcher.Get())
            return (ulong)obj["TotalVisibleMemorySize"] * 1024;

        return 0;
    }

    private static ulong GetTotalRamBytesLinux()
    {
        foreach (var line in File.ReadLines("/proc/meminfo"))
        {
            if (line.StartsWith("MemTotal:"))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return ulong.Parse(parts[1]) * 1024;
            }
        }

        throw new InvalidOperationException("MemTotal not found");
    }
}
