using System;
using Nix.Resources;

namespace Nix.MVC
{
    public class NixGuild : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"ID: {Id}\n" +
                $"Guild ID: {DiscordId}" +
                $"Created At: {CreatedAt}\n" +
                $"Stored At: {StoredAt}";
        }
    }
}
