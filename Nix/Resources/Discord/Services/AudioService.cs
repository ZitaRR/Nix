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
                await channel.SendMessageAsync("You must be connected to a voice-channel");
                return false;
            }
            if (lavaNode.HasPlayer(state.VoiceChannel.Guild))
            {
                if (playCommand)
                    return true;
                await channel.SendMessageAsync("I'm already connected to a voice-channel");
                return true;
            }

            try
            {
                this.channel = channel;
                await lavaNode.JoinAsync(state?.VoiceChannel);
                users.Clear();
                await channel.SendMessageAsync($"Joined {state?.VoiceChannel.Name}");
                return true;
            }
            catch(Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                return false;
            }
        }

        public async Task<bool> LeaveAsync(IVoiceState state, ITextChannel channel)
        {
            if (state?.VoiceChannel is null)
            {
                await channel.SendMessageAsync("You must be connected to a voice-channel");
                return false;
            }
            if (!lavaNode.HasPlayer(state?.VoiceChannel.Guild))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return true;
            }

            try
            {
                this.channel = null;
                await lavaNode.LeaveAsync(state?.VoiceChannel);
                users.Clear();
                await channel.SendMessageAsync($"Left {state?.VoiceChannel.Name}");
                return true;
            }
            catch(Exception e)
            {
                await channel.SendMessageAsync(e.Message);
                return false;
            }
        }

        public async Task PlayAsync(IVoiceState state, ITextChannel channel, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await channel.SendMessageAsync("No query was provided");
                return;
            }
            if (!await JoinAsync(state, channel, true))
                return;

            var response = await lavaNode.SearchAsync(search);
            if (response.LoadStatus == LoadStatus.LoadFailed ||
                response.LoadStatus == LoadStatus.NoMatches)
            {
                await channel.SendMessageAsync($"Invalid URL");
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
                await channel.SendMessageAsync("No query was provided");
                return;
            }

            var queries = search.Split(' ');
            foreach (var query in queries)
            {
                var response = await lavaNode.SearchYouTubeAsync(query);
                if (response.LoadStatus == LoadStatus.LoadFailed ||
                    response.LoadStatus == LoadStatus.NoMatches)
                {
                    await channel.SendMessageAsync($"No matches for {query}");
                    return;
                }

                await PlayAsync(state, channel, response.Tracks.FirstOrDefault().Url);
            }
        }

        public async Task DurationAsync(ITextChannel channel)
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped || player.Track is null)
            {
                await channel.SendMessageAsync("I'm not playing anything");
                return;
            }

            LavaTrack track = player.Track;
            await channel.SendMessageAsync($"{track.Position:mm\\:ss} / {track.Duration:mm\\:ss}");
        }

        public async Task SkipAsync(IVoiceState state, ITextChannel channel)
        {
            if (state?.VoiceChannel is null)
            {
                await channel.SendMessageAsync("You must be connected to a voice-channel");
                return;
            }
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped ||
                player.Track is null)
            {
                await channel.SendMessageAsync("Nothing is currently playing");
                return;
            }

            var track = player.Track;
            await reply.AudioSkipAsync(channel, track, player.Queue.Count);
            await player.StopAsync();
        }

        public async Task CurrentAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped || 
                player.Track is null)
            {
                await channel.SendMessageAsync("Nothing is currently playing");
                return;
            }

            LavaTrack track = player.Track;
            await reply.AudioCurrentlyPlayingAsync(channel, users.Peek(), track);
        }

        public async Task ArtworkAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected a voice-channel");
                return;
            }
            if (player.PlayerState != PlayerState.Playing ||
                player.Track is null)
            {
                await channel.SendMessageAsync("Nothing is currently playing");
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
                await channel.SendMessageAsync("No more tracks in the queue");
                return;
            }

            await reply.AudioQueueAsync(context, player.Queue.ToList(), player.Track);
        }

        public async Task RepeatAsync()
        {
            if (!lavaNode.TryGetPlayer(channel.Guild, out LavaPlayer player))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState == PlayerState.Stopped ||
                player.Track is null)
            {
                await channel.SendMessageAsync("Nothing is currently playing");
            }

            repeat = !repeat;
            string result = repeat is true ? "Repeat turned on" : "Repeat turned off";
            await channel.SendMessageAsync(result);
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
                await channel.SendMessageAsync("No more tracks in the queue");
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
