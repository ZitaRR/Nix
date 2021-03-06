using System;
using System.Collections.Generic;
using Nix.Resources;

namespace Nix.Models
{
    public class NixUser : IStorable
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ulong UserID { get; set; }
        public ulong GuildID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime StoredAt { get; set; }
        public IEnumerable<NixRole> Roles { get; set; }
        public string AvatarURL { get; set; }
        public int TotalMessages { get; set; }

        public override string ToString()
            => $"Name: {Name}\n" +
            $"ID: {ID}\n" + 
            $"User ID: {UserID}\n" +
            $"Guild ID: {GuildID}\n" +
            $"Created At: {CreatedAt}\n" +
            $"Joined At: {JoinedAt}\n" +
            $"Stored At: {StoredAt}\n" +
            $"Avatar URL: {AvatarURL}\n" +
            $"Messages Sent: {TotalMessages}";
    }
}
