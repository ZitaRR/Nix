using Nix.Resources;
using System;

namespace Nix.MVC
{
    public class NixTrack : IStorable
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string AudioUri { get; set; }
        public string SourceUri { get; set; }
        public DateTime StoredAt { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}\n" +
                $"Audio: {AudioUri}\n" +
                $"Source: {SourceUri}\n" +
                $"Stored At: {StoredAt}";
        }
    }
}
