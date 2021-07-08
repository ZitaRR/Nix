using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Victoria;
using Discord;

namespace Nix.MVC
{
    public sealed class NixPlayer 
    {
        public LavaPlayer Player { get; }
        public CancellationTokenSource Source { get; private set; }
        public CancellationToken Token { get; private set; }
        public IVoiceChannel VoiceChannel => Player.VoiceChannel;
        public ITextChannel TextChannel => Player.TextChannel;
        public LavaTrack CurrentTrack => Player.Track ?? null;
        public IList<LavaTrack> Queue => Player.Queue.ToList();
        public TimeSpan Position => CurrentTrack?.Position ?? TimeSpan.Zero;
        public int Volume => Player.Volume;
        public bool OnRepeat { get; set; }

        public NixPlayer(LavaPlayer player)
        {
            Player = player;
            OnRepeat = false;
            Source = new CancellationTokenSource();
            Token = Source.Token;
        }

        public void ResetCancellation()
        {
            Source.Dispose();
            Source = new CancellationTokenSource();
            Token = Source.Token;
        }
    }
}
