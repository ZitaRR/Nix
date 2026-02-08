using Nix.Core.Shl.Teams.Private;
using System;
using System.Collections.Generic;

namespace Nix.Core.Shl.Teams;

 public record Team(
     string Id,
     string Code,
     string LongName,
     string FullName,
     Uri IconUri,
     IEnumerable<Player> Players)
{
    internal static Team Create(TeamDto dto, IEnumerable<Player> players) =>
        new(
            dto.Id, 
            dto.Names.Code,
            dto.Names.Long,
            dto.Names.Full,
            new Uri(dto.IconUrl),
            players);
}
