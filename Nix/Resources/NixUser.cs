using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nix.Resources
{
    public class NixUser
    {
        public string Name { get; set; }
        public ulong ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<IRole> Roles { get; set; }
        public string AvatarURL { get; set; }
        public ConcurrentBag<DateTime> PlusReps { get; set; } = new ConcurrentBag<DateTime>();
        public ConcurrentBag<DateTime> MinusReps { get; set; } = new ConcurrentBag<DateTime>();
    }
}
