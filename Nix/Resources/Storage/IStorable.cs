using System;

namespace Nix.Resources
{
    public interface IStorable
    {
        public int Id { get; }
        public string DiscordId { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
