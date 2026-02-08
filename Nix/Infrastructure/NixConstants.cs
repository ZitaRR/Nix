using System;
using System.IO;

namespace Nix.Infrastructure;

internal static class NixConstants
{
    internal const string NIX = "Nix";
    internal const string DISCORD_TOKEN = nameof(DISCORD_TOKEN);

    internal static readonly string LogoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
    internal static readonly string MatchCardTemplatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "MatchCardTemplate.html");
    internal static readonly string LiveStatusPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "LiveStatus.html");
    internal static readonly string EndedStatusPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "EndedStatus.html");
    internal static readonly string StandingsTemplatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "StandingsTemplate.html");
    internal static readonly string StandingsTeamTemplatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "StandingsTeamTemplate.html");
    internal static readonly string SystemMonitorTemplatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "HtmlTemplates", "SystemMonitorTemplate.html");
    internal static readonly string StylingPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Playwright", "Styling", "Styles.css");
}
