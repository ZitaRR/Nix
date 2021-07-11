using System;
using System.Collections.Generic;
using Nix.Resources;

namespace Nix.MVC
{
    public class NixEvent : IStorable
    {
        public string Id { get; set; }
        public string DiscordId { get; set; }
        public long GuildID { get; set; }
        public long MessageID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public NixUser Creator { get; set; }
        public List<NixUser> Participants { get; set; } = new List<NixUser>();
        public List<NixUser> PossibleParticipants { get; set; } = new List<NixUser>();
        public DateTime StoredAt { get; set; }
        public bool SentNotice { get; set; }

        public override string ToString()
        {
            var users = "";
            foreach (var user in Participants)
            {
                users += $"\n          {user.Name}";
            }
            return $"Name: {Name}\n" +
            $"ID: {Id}\n" +
            $"Guild ID: {GuildID}\n" +
            $"Message ID: {MessageID}\n" + 
            $"Start: {Start.ToLongTimeString()}\n" + 
            $"Stored At: {StoredAt}\n" +
            $"Users: {users}";
        }
    }
}
