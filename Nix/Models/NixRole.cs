using System;
using Nix.Resources;

namespace Nix.Models
{
    public class NixRole : IStorable
    {
        public int ID { get; set; }
        public ulong RoleID { get; set; }
        public string Name { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
