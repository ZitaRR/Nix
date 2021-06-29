using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixUser : IStorable
    {
        public string Id { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public int Messages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
            => $"ID: {Id}\n" +
            $"Guild ID: {GuildId}\n" +
            $"Username: {Name}\n" +
            $"Avatar URL: {AvatarUrl}" +
            $"Messages: {Messages}" +
            $"Created At: {CreatedAt}\n" +
            $"Joined At: {JoinedAt}\n" +
            $"Stored At: {StoredAt}\n";
    }
}
