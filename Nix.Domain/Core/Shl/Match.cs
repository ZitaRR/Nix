using System;

namespace Nix.Domain.Core.Shl;

public record Match(
    DateTime StartDateTime,
    Team HomeTeam, 
    Team AwayTeam);
