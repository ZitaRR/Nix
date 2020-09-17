using System;
using System.Collections.Generic;

namespace Nix.Resources
{
    public class NixGuild : IStorable
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ulong GuildID { get; set; }
        public List<NixUser> Users { get; set; }
        public List<NixChannel> Channels { get; set; }
        public DateTime StoredAt { get; } = DateTime.Now;
    }
}
