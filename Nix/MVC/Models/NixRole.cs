using System;
using Nix.Resources;

namespace Nix.MVC
{
    public class NixRole : IStorable
    {
        public string Id { get; set; }
        public string DiscordId { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
