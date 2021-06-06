using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixUser : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime StoredAt { get; set; }
        public string AvatarUrl { get; set; }
        public int TotalMessages { get; set; }

        public override string ToString()
            => $"ID: {Id}\n" + 
            $"User ID: {DiscordId}\n" +
            $"Guild ID: {GuildId}\n" +
            $"Username: {Name}\n" +
            $"Created At: {CreatedAt}\n" +
            $"Joined At: {JoinedAt}\n" +
            $"Stored At: {StoredAt}\n" +
            $"Avatar URL: {AvatarUrl}\n" +
            $"Messages Sent: {TotalMessages}";
    }
}
