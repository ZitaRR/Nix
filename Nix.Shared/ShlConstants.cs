using System;
using System.IO;

namespace Nix.Shared;

public static class ShlConstants
{
    public const string CLIENT = "SHL_CLIENT";
    public const string BASE_URL = "https://www.shl.se/";
    public const string SETTINGS_ENDPOINT = "api/site/settings";
    public const string SEASONS_ENDPOINT = "api/sports-v2/season-series-game-types-filter";
    public const string GAME_INFO_ENDPOINT = "api/sports-v2/game-schedule?seasonUuid={0}&seriesUuid=qQ9-bb0bzEWUk&gameTypeUuid={1}&gamePlace=all&played=all";
    public const string UPCOMING_MATCHES_ENDPOINT = "api/sports-v2/upcoming-live-games";
    public const string GAME_OVERVIEW_ENDPOINT = "api/gameday/game-overview/";
    public const string TEAMS_ENDPOINT = "api/sports-v2/teams/";
    public const string PLAYERS_ENDPOINT = "api/sports-v2/athletes/by-team-uuid/";
    public const string STANDINGS_ENDPOINT = "api/statistics-v2/stats-info/standings_standings?ssgtUuid=";

    public static readonly string Shl2026LogosPath = Path.Combine(AppContext.BaseDirectory, "Assets", "SHL 2026");
}
