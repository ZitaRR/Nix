using System;
using System.Collections.Generic;
using Nix.Resources;

namespace Nix.Models
{
    public class NixGuild : IStorable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
