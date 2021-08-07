using Nix.Resources;
using System;

namespace Nix.MVC.Models
{
    public class NixTrack : IStorable
    {
        public int Id { get; set; }
        public DateTime StoredAt { get; set; }
    }
}
