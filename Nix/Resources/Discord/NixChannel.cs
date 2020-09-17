using System;

namespace Nix.Resources
{
    public class NixChannel : IStorable
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ulong ChannelID { get; set; }
        public ulong GuildID { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
            => $"Name: {Name}\n" +
            $"ID: {ID}\n" +
            $"Channel ID: {ChannelID}\n" +
            $"Guild ID: {GuildID}\n" +
            $"Stored At: {StoredAt}";
    }
}
