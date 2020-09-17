using System;

namespace Nix.Resources
{
    public class NixRole : IStorable
    {
        public int ID { get; set; }
        public ulong RoleID { get; set; }
        public string Name { get; set; }
        public DateTime StoredAt { get; } = DateTime.Now;
    }
}
