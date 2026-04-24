using Nix.Domain.Core.Shl;
using System.Collections.Generic;
using System.Linq;

namespace Nix.Bot;

public static class ShlExtensions
{
    public static string ToPlayerString(this Player player) =>
        $"**#{player.Number}** | {player.FirstName} {player.Surname}";

    public static string ListPlayers(this IEnumerable<Player> players) =>
        string.Join("\n", players.Select(ToPlayerString));
}
