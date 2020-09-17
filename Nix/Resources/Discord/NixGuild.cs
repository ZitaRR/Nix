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
        public int TextChannels { get; set; }
        public int VoiceChannels { get; set; }
        public int Channel { get => TextChannels + VoiceChannels; }
        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
