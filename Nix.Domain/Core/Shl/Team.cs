using System.Collections.Immutable;

namespace Nix.Domain.Core.Shl;

public record Team(
    string Id, 
    string Name, 
    string Code, 
    Standing Standing, 
    ImmutableArray<Player> Players);
