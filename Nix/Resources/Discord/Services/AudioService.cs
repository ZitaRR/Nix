using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
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

        public AudioService(LavaNode lavaNode, ReplyService reply)
        {
            this.lavaNode = lavaNode;
            this.reply = reply;
            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStarted += OnTrackStart;
            users = new Queue<SocketGuildUser>();
        }

        public async Task JoinAsync(IVoiceState state, ITextChannel channel)
        {
            if (state?.VoiceChannel is null)
                await channel.SendMessageAsync("You must be connected to a voice-channel");
            if (lavaNode.HasPlayer(state.VoiceChannel.Guild))
                await channel.SendMessageAsync("I'm already connected to a voice-channel");

            try
            {
                this.channel = channel;
                await lavaNode.JoinAsync(state?.VoiceChannel);
                users.Clear();
                await channel.SendMessageAsync($"Joined {state?.VoiceChannel.Name}");
            }
            catch(Exception e)
            {
                await channel.SendMessageAsync(e.Message);
            }
        }

        public async Task LeaveAsync(IVoiceState state, ITextChannel channel)
        {
            if (state?.VoiceChannel is null)
                await channel.SendMessageAsync("You must be connected to a voice-channel");
            if (!lavaNode.HasPlayer(state?.VoiceChannel.Guild))
                await channel.SendMessageAsync("I'm not connected to a voice-channel");

            try
            {
                this.channel = null;
                await lavaNode.LeaveAsync(state?.VoiceChannel);
                users.Clear();
                await channel.SendMessageAsync($"Left {state?.VoiceChannel.Name}");
            }
            catch(Exception e)
            {
                await channel.SendMessageAsync(e.Message);
            }
        }

        public async Task PlayAsync(IVoiceState state, ITextChannel channel, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await channel.SendMessageAsync("No query was provided");
                return;
            }
            if (!lavaNode.HasPlayer(state?.VoiceChannel.Guild))
            {
                await channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }

            var queries = search.Split(' ');
            foreach (var query in queries)
            {
                var response = await lavaNode.SearchAsync(query);
                if (response.LoadStatus == LoadStatus.LoadFailed ||
                    response.LoadStatus == LoadStatus.NoMatches)
                {
                    await channel.SendMessageAsync($"No matches for {query}");
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
                            {
                                await player.PlayAsync(track);
                                users.Enqueue(state as SocketGuildUser);
                            }
                            else player.Queue.Enqueue(response.Tracks[i]);
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
            await channel.SendMessageAsync($"Skipped: {track.Title}");
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
            await channel.SendMessageAsync($"Now Playing: {track.Title}\n" +
                $"Requested By: {users.Peek().Username}\n" +
                $"Duration: {track.Position:mm\\:ss} / {track.Duration:mm\\:ss}");
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

        private async Task OnTrackEnd(TrackEndedEventArgs args)
        {
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

            await reply.AudioPlayAsync(channel, users.Peek(), args.Track);
        }
    }
}
