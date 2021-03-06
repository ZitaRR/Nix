using System;
using System.Collections.Generic;
using Nix.Resources;

namespace Nix.Models
{
    public class NixGuild : IStorable
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ulong GuildID { get; set; }
        public List<NixUser> Users { get; set; }
        public List<NixChannel> Channels { get; set; }
        public DateTime StoredAt { get; set; }
        public NixUser Client { get; set; }
    }
}
