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
        public bool OnRepeat { get; }

        public LavalinkData(NixPlayer player)
        {
            CurrentTrack = player.CurrentTrack ?? null;
            Queue = player.Queue.ToList();
            VoiceChannel = player.VoiceChannel;
            TextChannel = player.TextChannel;
            Position = CurrentTrack?.Position ?? TimeSpan.Zero;
            Volume = player.Volume;
            OnRepeat = player.OnRepeat;
        }
    }
}
