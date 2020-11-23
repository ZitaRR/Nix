using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Nix.Resources.Discord
{
    public sealed class AudioService
    {
        private readonly LavaNode lavaNode;
        private readonly ReplyService reply;
        private readonly ushort defaultVolume = 50;
        private ITextChannel channel;
        private Queue<SocketGuildUser> users;
        private bool repeat = false;

        public AudioService(LavaNode lavaNode, ReplyService reply)
        {
            this.lavaNode = lavaNode;
            this.reply = reply;
            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStarted += OnTrackStart;
            users = new Queue<SocketGuildUser>();
        }

        public async Task<bool> JoinAsync(IVoiceState state, ITextChannel channel, bool playCommand = false)
        {
            if (state?.VoiceChannel is null)
            {
                await reply.ErrorAsync(channel, "You must be connected to a voice-channel");
                return false;
            }
            if (lavaNode.TryGetPlayer(state.VoiceChannel.Guild, out LavaPlayer player))
            {
                if (playCommand)
                    return true;
                await reply.ErrorAsync(channel, $"I'm already connected to {player.VoiceChannel.Name}");
                return true;
            }

            try
            {
                this.channel = channel;
                await lavaNode.JoinAsync(state?.VoiceChannel);
                await player.UpdateVolumeAsync(defaultVolume);
                users.Clear();
                await reply.MessageAsync(channel, $"Joined {state?.VoiceChannel.Name}");
                return true;
            }
            catch(Exception e)
            {
                await reply.ExceptionAsync(channel, e);
                return false;
            }
        }

        public async Task<bool> LeaveAsync(IVoiceState state, ITextChannel channel)
        {
            if (state?.VoiceChannel is null)
            {
                await reply.ErrorAsync(channel, "You must be connected to a voice-channel");
                return false;
            }
            if (!lavaNode.HasPlayer(state?.VoiceChannel.Guild))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return true;
            }

            try
            {
                this.channel = null;
                await lavaNode.LeaveAsync(state?.VoiceChannel);
                users.Clear();
                await reply.MessageAsync(channel, $"Left {state?.VoiceChannel.Name}");
                return true;
            }
            catch(Exception e)
            {
                await reply.ExceptionAsync(channel, e);
                return false;
            }
        }

        public async Task PlayAsync(IVoiceState state, ITextChannel channel, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await reply.ErrorAsync(channel, "No query was provided");
                return;
            }
            if (!await JoinAsync(state, channel, true))
                return;

            var response = await lavaNode.SearchAsync(search);
            if (response.LoadStatus == LoadStatus.LoadFailed ||
                response.LoadStatus == LoadStatus.NoMatches)
            {
                await reply.ErrorAsync(channel, "Invalid URL");
                return;
            }

            var player = lavaNode.GetPlayer(state?.VoiceChannel.Guild);
            this.channel = channel;
            if (player.PlayerState == PlayerState.Playing ||
                player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(response.Playlist.Name))
                {
                    foreach (var track in response.Tracks)
                    {
                        player.Queue.Enqueue(track);
                        users.Enqueue(state as SocketGuildUser);
                    }

                    await reply.AudioEnqueueManyAsync(channel, response.Tracks);
                }
                else
                {
                    var track = response.Tracks[0];
                    player.Queue.Enqueue(track);
                    users.Enqueue(state as SocketGuildUser);
                    await reply.AudioEnqueueSingleAsync(channel, track);
                }
            }
            else
            {
                var track = response.Tracks[0];
                if (!string.IsNullOrWhiteSpace(response.Playlist.Name))
                {
                    for (int i = 0; i < response.Tracks.Count; i++)
                    {
                        if (i == 0)
                            await player.PlayAsync(track);
                        else player.Queue.Enqueue(response.Tracks[i]);
                        users.Enqueue(state as SocketGuildUser);
                    }

                    await reply.AudioEnqueueManyAsync(channel, response.Tracks);
                }
                else
                {
                    await player.PlayAsync(track);
                    users.Enqueue(state as SocketGuildUser);
                }
            }
        }

        public async Task PlayYoutubeAsync(IVoiceState state, ITextChannel channel, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await reply.ErrorAsync(channel, "No query was provided");
                return;
            }

            var queries = search.Split(' ');
            foreach (var query in queries)
            {
                var response = await lavaNode.SearchYouTubeAsync(query);
                if (response.LoadStatus == LoadStatus.LoadFailed ||
                    response.LoadStatus == LoadStatus.NoMatches)
                {
                    await reply.ErrorAsync(channel, $"No matches for {query}");
                    return;
                }

                await PlayAsync(state, channel, response.Tracks.FirstOrDefault().Url);
            }
        }

        public async Task DurationAsync(ITextChannel channel)
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to any vice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped || player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            LavaTrack track = player.Track;
            await reply.MessageAsync(channel, $"{track.Position:mm\\:ss} / {track.Duration:mm\\:ss}");
        }

        public async Task SkipAsync(IVoiceState state, ITextChannel channel, int amount)
        {
            if (state?.VoiceChannel is null)
            {
                await reply.ErrorAsync(channel, "You must be connected to a voice-channel");
                return;
            }
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped ||
                player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            var track = player.Track;
            if (amount == 1)
                await reply.AudioSkipAsync(channel, track, player.Queue.Count);
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    if (!player.Queue.TryDequeue(out LavaTrack _))
                    {
                        await reply.AudioSkipAsync(channel, i, player.Queue.Count);
                        break;
                    }
                    else if (i == amount - 1)
                    {
                        await reply.AudioSkipAsync(channel, i + 1, player.Queue.Count);
                        break;
                    }
                }
            }

            await player.StopAsync();
        }

        public async Task CurrentAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped || 
                player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            LavaTrack track = player.Track;
            await reply.AudioCurrentlyPlayingAsync(channel, users.Peek(), track, player.Volume, repeat);
        }

        public async Task ArtworkAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState != PlayerState.Playing ||
                player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            string artwork = await player.Track.FetchArtworkAsync();
            await channel.SendMessageAsync(artwork);
        }

        public async Task ListQueueAsync(NixCommandContext context)
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.Queue.Count <= 0)
            {
                await reply.ErrorAsync(channel, "No more tracks in the queue");
                return;
            }

            await reply.AudioQueueAsync(context, player.Queue.ToList(), player.Track);
        }

        public async Task RepeatAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }

            repeat = !repeat;
            string result = repeat is true ? "Repeat turned on" : "Repeat turned off";
            await reply.MessageAsync(channel, result);
        }

        public async Task VolumeAsync(IVoiceState state, ushort volume)
        {
            if (state?.VoiceChannel is null)
            {
                await reply.ErrorAsync(channel, "You must be connected to a voice-channel");
                return;
            }
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped ||
                player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            var previous = player.Volume;
            await player.UpdateVolumeAsync(volume);
            await reply.MessageAsync(channel, $"Changed volume to {player.Volume} from {previous}");
        }

        private async Task OnTrackEnd(TrackEndedEventArgs args)
        {
            if (repeat)
            {
                await args.Player.PlayAsync(args.Track);
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var track))
            {
                channel = null;
                await reply.ErrorAsync(channel, "No more tracks in the queue");
                await lavaNode.LeaveAsync(args.Player.VoiceChannel);
                users.Clear();
                return;
            }

            users.Dequeue();
            await args.Player.PlayAsync(track);
        }

        private async Task OnTrackStart(TrackStartEventArgs args)
        {
            if (args.Player.PlayerState != PlayerState.Playing ||
                args.Track is null)
            {
                users.Clear();
                return;
            }

            await reply.AudioPlayAsync(channel, args.Track);
        }
    }
}
