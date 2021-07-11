using System;

namespace Nix.Resources
{
    public interface IStorable
    {
        public string Id { get; }
        public DateTime StoredAt { get; set; }
    }
}
