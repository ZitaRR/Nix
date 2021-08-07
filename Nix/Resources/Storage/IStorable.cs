using System;

namespace Nix.Resources
{
    public interface IStorable
    {
        public int Id { get; }
        public DateTime StoredAt { get; set; }
    }
}
