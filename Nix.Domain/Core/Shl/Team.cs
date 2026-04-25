using System.Collections.Immutable;
using System.IO;

namespace Nix.Domain.Core.Shl;

public record Team(
    string Id, 
    string Name, 
    string Code, 
    byte[] IconBytes,
    Standing Standing, 
    ImmutableArray<Player> Players);
