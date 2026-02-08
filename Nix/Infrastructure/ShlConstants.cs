using System;
using System.IO;

namespace Nix.Infrastructure;

internal static class ShlConstants
{
    internal const string CLIENT = "SHL_CLIENT";
    internal const string BASE_URL = "https://www.shl.se/";
    internal const string GAME_INFO_ENDPOINT = "api/sports-v2/game-schedule?seasonUuid=xs4m9qupsi&seriesUuid=qQ9-bb0bzEWUk&gameTypeUuid=qQ9-af37Ti40B&gamePlace=all&played=all";
    internal const string UPCOMING_MATCHES_ENDPOINT = "api/sports-v2/upcoming-live-games";
    internal const string GAME_OVERVIEW_ENDPOINT = "api/gameday/game-overview/";
    internal const string TEAMS_ENDPOINT = "api/sports-v2/teams/";
    internal const string PLAYERS_ENDPOINT = "api/sports-v2/athletes/by-team-uuid/";
    internal const string STANDINGS_ENDPOINT = "api/statistics-v2/stats-info/standings_standings?ssgtUuid=";

    internal static readonly string Shl2026LogosPath = Path.Combine(AppContext.BaseDirectory, "Assets", "SHL 2026");
}
