using Victoria;
using System.Threading.Tasks;
using System;
using System.Linq;
using Discord;
using Nix.Resources.Discord;
using Victoria.Enums;
using Victoria.EventArgs;
using System.Threading;
using Victoria.Responses.Rest;

namespace Nix.Resources
{
    public sealed class NixPlayer 
    {
        public LavaTrack[] Queue { get => player.Queue.ToArray(); }
        public LavaTrack CurrentTrack { get => player.Track; }
        public IVoiceChannel VoiceChannel { get => player.VoiceChannel; }
        public ITextChannel TextChannel { get => player.TextChannel; }
        public int Volume { get => player.Volume; }
        public bool IsPlaying { get => player.PlayerState is PlayerState.Playing; }
        public bool IsPaused { get => player.PlayerState is PlayerState.Paused; }
        public bool OnRepeat { get; set; }

        private readonly LavaNode lavaNode;
        private readonly SpotifyService spotify;
        private readonly AudioService audio;
        private readonly TimeSpan inactivity = TimeSpan.FromMinutes(1);
        private readonly ushort defaultVolume = 50;
        private readonly int titleLength = 40;
        private LavaPlayer player;
        private CancellationTokenSource source = new CancellationTokenSource();

        public NixPlayer(LavaNode lavaNode, SpotifyService spotify, AudioService audio)
        {
            this.lavaNode = lavaNode;
            this.spotify = spotify;
            this.audio = audio;

            this.lavaNode.OnTrackStarted += OnTrackStart;
            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStuck += OnTrackStuck;
            this.lavaNode.OnTrackException += OnTrackException;
        }

        public async Task<bool> JoinAsync(IVoiceChannel voice, ITextChannel text = null)
        {
            if (text is null)
            {
                text = TextChannel;
            }

            await lavaNode.JoinAsync(voice, text)
                .ContinueWith(async (_player) =>
                {
                    player = await _player;
                    await SetVolumeAsync(defaultVolume);
                });

            return true;
        }

        public async Task LeaveAsync()
        {
            await audio.RemovePlayerFromGuildAsync(VoiceChannel.Guild);
            await lavaNode.LeaveAsync(VoiceChannel);
        }

        public async Task<LavaTrack[]> PlayAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new LavaTrack[] { };
            }
            if (spotify.IsSpotifyUri(query))
            {
                return await PlaySpotifyAsync(query);
            }

            SearchResponse response;
            if (Uri.IsWellFormedUriString(query, UriKind.Absolute))
            {
                response = await lavaNode.SearchAsync(query);
            }
            else
            {
                response = await lavaNode.SearchYouTubeAsync(query);
            }

            if (response.LoadStatus is LoadStatus.LoadFailed ||
                response.LoadStatus is LoadStatus.NoMatches)
            {
                return new LavaTrack[] { };
            }

            int index = Queue.Length;
            if (!string.IsNullOrEmpty(response.Playlist.Name))
            {
                foreach (LavaTrack track in response.Tracks)
                {
                    if (IsPlaying || IsPaused)
                    {
                        player.Queue.Enqueue(track);
                        continue;
                    }

                    await player.PlayAsync(track);
                }
            }
            else
            {
                LavaTrack track = response.Tracks[0];
                if (IsPlaying || IsPaused)
                {
                    player.Queue.Enqueue(track);
                }
                else
                {
                    await player.PlayAsync(track);
                    return new LavaTrack[] { track };
                }
            }

            int length = player.Queue.Count - index;
            var array = new LavaTrack[length];
            Array.Copy(Queue, index, array, 0, length);
            return array;
        }

        public async Task PlayAsync(LavalinkData data)
        {
            await SetVolumeAsync(data.Volume);
            await player.PlayAsync(data.CurrentTrack);
            await SeekAsync(data.Position);

            foreach (LavaTrack track in data.Queue)
            {
                player.Queue.Enqueue(track);
            }
        }

        public async Task<LavaTrack[]> PlaySpotifyAsync(string uri)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SkipAsync(int amount)
        {
            int skipped = 1;
            if (amount > 1)
            {
                for (int i = 0; i < amount - 1; i++)
                {
                    if (player.Queue.TryDequeue(out _))
                    {
                        skipped++;
                    }
                }
            }

            await player.StopAsync();
            return skipped;
        }

        public async Task<string> GetArtworkAsync()
        {
            return await player.Track.FetchArtworkAsync();
        }

        public async Task SetVolumeAsync(ushort volume)
        {
            await player.UpdateVolumeAsync(volume);
        }

        public async Task SeekAsync(TimeSpan time)
        {
            time = time > player.Track.Duration ? player.Track.Duration : time;
            await player.SeekAsync(time);
        }

        public async Task<bool> PauseAsync()
        {
            if (IsPaused)
            {
                return false;
            }

            await player.PauseAsync();
            return true;
        }

        public async Task<bool> ResumeAsync()
        {
            if (IsPlaying)
            {
                return false;
            }

            await player.ResumeAsync();
            return true;
        }

        public async Task<string> GetLyricsAsync()
        {
            var lyrics = await player.Track.FetchLyricsFromGeniusAsync();
            if (string.IsNullOrEmpty(lyrics))
            {
                lyrics = $"Could not find any lyrics for {player.Track.Title}";
            }
            return lyrics;
        }

        public Task Shuffle()
        {
            player.Queue.Shuffle();
            return Task.CompletedTask;
        }

        public async Task InitiateDisconnectAsync()
        {
            await TextChannel.SendMessageAsync($"Leaving in {inactivity:m\\:ss}");
            bool cancelled = SpinWait.SpinUntil(() => source.Token.IsCancellationRequested, inactivity);

            if (cancelled)
            {
                await ResetCancellationAsync();
                return;
            }

            await TextChannel.SendMessageAsync("Leaving due to inactivity");
            await LeaveAsync();
        }

        public Task CancelDisconnectAsync()
        {
            TextChannel.SendMessageAsync($"Disconnection cancelled");
            source.Cancel();
            return Task.CompletedTask;
        }

        private Task ResetCancellationAsync()
        {
            source.Dispose();
            source = new CancellationTokenSource();
            return Task.CompletedTask;
        }

        public Task<string> FormatTitleAsync(string title)
        {
            int index = title.IndexOfAny(new char[] { '(', '[' });
            if (index != -1)
            {
                title = title.Substring(0, index);
            }

            if (title.Length > titleLength)
            {
                title = title.Substring(0, titleLength);
                if (char.IsWhiteSpace(title[^1]))
                {
                    title = title[0..^1] + "...";
                }
                else
                {
                    title = title + "...";
                }
            }
            return Task.FromResult(title);
        }

        public async Task<string> GetTitleAsUrlAsync(LavaTrack track)
        {
            return $"[{await FormatTitleAsync(track.Title)}]({track.Url})";
        }

        private async Task OnTrackStart(TrackStartEventArgs arg)
        {
            await TextChannel.SendMessageAsync($"**Now Playing** {CurrentTrack.Title}");
        }

        private async Task OnTrackEnd(TrackEndedEventArgs arg)
        {
            if (OnRepeat)
            {
                await player.PlayAsync(arg.Track);
                return;
            }

            if (!player.Queue.TryDequeue(out LavaTrack track))
            {
                await TextChannel.SendMessageAsync($"No more tracks in the queue");
                await InitiateDisconnectAsync();
                return;
            }

            await player.PlayAsync(track);
        }

        private async Task OnTrackStuck(TrackStuckEventArgs arg)
        {
            await TextChannel.SendMessageAsync($"{CurrentTrack.Title} got stuck");
        }

        private async Task OnTrackException(TrackExceptionEventArgs arg)
        {
            await TextChannel.SendMessageAsync($"{CurrentTrack} threw an exception");
        }
    }
}
