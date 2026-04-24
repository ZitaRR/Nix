using System;

namespace Nix.Domain.Core.Shl;

public record Player(string FirstName, string Surname, int Number, Position Position, Uri PortraitUri)
{
    public override string ToString() =>
        $"# {Number}\u2007\u2007{FirstName} {Surname}";
}
