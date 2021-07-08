using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixChannel : IStorable
    {
        public string Id { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
            => $"Name: {Name}\n" +
            $"ID: {Id}\n" +
            $"Guild ID: {GuildId}\n" +
            $"Created At: {CreatedAt}\n" +
            $"Stored At: {StoredAt}";
    }
}
