using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using Victoria;

namespace Nix.Resources.Discord
{
    public struct LavalinkData
    {
        public LavaTrack CurrentTrack { get; }
        public IList<LavaTrack> Queue { get; }
        public IVoiceChannel VoiceChannel { get; }
        public ITextChannel TextChannel { get; }
        public TimeSpan Position { get; }
        public int Volume { get; }

        public LavalinkData(LavaPlayer player)
        {
            CurrentTrack = player.Track ?? null;
            Queue = player.Queue.ToList();
            VoiceChannel = player.VoiceChannel;
            TextChannel = player.TextChannel;
            Position = player.Track?.Position ?? TimeSpan.Zero;
            Volume = player.Volume;
        }
    }
}
