using Discord;
using Discord.Commands;
using Nix.Infrastructure;
using Nix.Rendering;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nix.Core.Discord.Modules;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    private readonly IRenderEngine renderer;
    private readonly string monitorTemplate;
    private readonly string css;

    public InfoModule(IRenderEngine renderer)
    {
        this.renderer = renderer;
        monitorTemplate = File.ReadAllText(NixConstants.SystemMonitorTemplatePath);
        css = File.ReadAllText(NixConstants.StylingPath);
    }

    [Command("info")]
    [Summary($"Provides info about {NixConstants.NIX}.")]
    public async Task InfoAsync()
    {
        await ReplyAsync(
            $"..::** INFO **::..\n" +
            $"Machine:              {Environment.MachineName}\n" +
            $"OS Version:           {Environment.OSVersion}\n" +
            $"OS Description:       {RuntimeInformation.OSDescription}\n" +
            $"OS Architecture:      {RuntimeInformation.OSArchitecture}\n" +
            $"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
    }

    [Command("monitor")]
    [Summary("Displays a system monitor.")]
    public async Task SystemMonitorAsync()
    {
        var html = CreateSystemMonitor();
        var stream = await renderer.RenderSingleAsync(html, css, ".system-card-compact");
        await Context.Channel.SendFileAsync(stream, $"{Guid.NewGuid()}.png");
    }

    private string CreateSystemMonitor()
    {
        var process = Process.GetCurrentProcess();
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        var used = Machine.GetUsedRamInGb();
        var total = Machine.GetTotalRamInGb();
        var percentage = (int)(used / total * 100);

        return monitorTemplate
            .Replace("{OS}", RuntimeInformation.OSDescription)
            .Replace("{CPU}", Machine.GetCpu())
            .Replace("{ARCHITECTURE}", RuntimeInformation.OSArchitecture.ToString())
            .Replace("{UPTIME}", $"{(int)double.Floor(uptime.TotalHours)}h {uptime.Minutes}m")
            .Replace("{USED_MEMORY}", used.ToString("F1"))
            .Replace("{TOTAL_MEMORY}", total.ToString("F1"))
            .Replace("{BAR}", percentage.ToString());
    }
}
