using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixChannel : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
            => $"Name: {Name}\n" +
            $"ID: {Id}\n" +
            $"Channel ID: {DiscordId}\n" +
            $"Guild ID: {GuildId}\n" +
            $"Stored At: {StoredAt}";
    }
}
