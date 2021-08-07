using System;
using Nix.Resources;

namespace Nix.MVC
{
    public class NixUser : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public int Messages { get; set; }
        public int CommandsIssued { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" + 
                $"ID: {Id}\n" +
                $"Discord ID: {DiscordId}\n" +
                $"Guild ID: {GuildId}\n" +
                $"Avatar URL: {AvatarUrl}\n" +
                $"Messages: {Messages}\n" +
                $"Commands Issued: {CommandsIssued}\n" +
                $"Created At: {CreatedAt}\n" +
                $"Joined At: {JoinedAt}\n" +
                $"Stored At: {StoredAt}";
        }
    }
}
