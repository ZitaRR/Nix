using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixGuild : IStorable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"ID: {Id}\n" +
                $"Created At: {CreatedAt}\n" +
                $"Stored At: {StoredAt}";
        }
    }
}
