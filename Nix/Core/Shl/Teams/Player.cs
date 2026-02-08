using Nix.Core.Shl.Teams.Private;
using System;

namespace Nix.Core.Shl.Teams;

public record Player(
    string Id,
    string FirstName,
    string Surname,
    int Number,
    Position Position,
    Uri PortraitUri)
{
    public static Player Create(PlayerDto player, Position position) =>
        new(
            player.Id,
            player.FirstName,
            player.Surname,
            player.Number,
            position,
            new Uri(player.Portrait.Url));
}
