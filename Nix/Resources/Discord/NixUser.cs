﻿using System;
using System.Collections.Generic;

namespace Nix.Resources
{
    public class NixUser : IStorable
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ulong UserID { get; set; }
        public ulong GuildID { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime CreatedAt { get; } = DateTime.Now;
        public IEnumerable<NixRole> Roles { get; set; }
        public string AvatarURL { get; set; }
        public int TotalMessages { get; set; }

        public override string ToString()
            => $"Name: {Name}\n" +
            $"ID: {ID}\n" + 
            $"User ID: {UserID}\n" +
            $"Guild ID: {GuildID}\n" +
            $"Created At: {CreatedAt}\n" +
            $"Messages Sent: {TotalMessages}";
    }
}
