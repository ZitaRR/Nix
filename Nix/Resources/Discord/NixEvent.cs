using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixEvent : IStorable
    {
        public int ID { get; set; }
        public ulong GuildID { get; set; }
        public ulong MessageID { get; set; }
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
            $"ID: {ID}\n" +
            $"Guild ID: {GuildID}\n" +
            $"Message ID: {MessageID}\n" + 
            $"Start: {Start.ToLongTimeString()}\n" + 
            $"Stored At: {StoredAt}\n" +
            $"Users: {users}";
        }
    }
}
