using System;
using System.Collections.Generic;
using Nix.Resources;

namespace Nix.Models
{
    public class NixGuild : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string Name { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
