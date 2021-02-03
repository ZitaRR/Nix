using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Rest;

namespace Nix.Resources.Discord
{
    public sealed class AudioService
    {
        private readonly LavaNode lavaNode;
        private readonly EmbedService reply;
        private readonly ushort defaultVolume = 50;
        private ITextChannel channel;
        private Queue<SocketGuildUser> users = new Queue<SocketGuildUser>();
        private bool repeat = false;
        private LavaPlayer player;

        public AudioService(LavaNode lavaNode, EmbedService reply, ScriptService script)
        {
            Task.Run(async () => await script.RunScript("run_lavalink.ps1"));
            this.lavaNode = lavaNode;
            this.reply = reply;
            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStarted += OnTrackStart;
        }

        public async Task<bool> JoinAsync(IVoiceState state, ITextChannel channel)
        {
            if (!(player is null))
            {
                await reply.ErrorAsync(channel, $"I'm already connected to {player.VoiceChannel.Name}");
                return true;
            }

            this.channel = channel;

            try
            {
                await lavaNode.JoinAsync(state?.VoiceChannel)
                    .ContinueWith(async (t) =>
                    {
                        player = await t;
                        await player.UpdateVolumeAsync(defaultVolume);
                    });
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

        public async Task<bool> LeaveAsync(IVoiceState state)
        {
            if (player is null)
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return true;
            }

            try
            {
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
            finally
            {
                channel = null;
                player = null;
            }
        }

        public async Task PlayAsync(IVoiceState state, ITextChannel channel, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                await reply.ErrorAsync(channel, "No query was provided");
                return;
            }
            if (!await JoinAsync(state, channel))
                return;

            var response = await SearchAsync(search);
            if (response.LoadStatus == LoadStatus.LoadFailed ||
                response.LoadStatus == LoadStatus.NoMatches)
            {
                await reply.ErrorAsync(channel, "No matches");
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

                    await reply.MessageAsync(channel,
                        $"**Enqueued** ``{response.Tracks.Count} tracks``\n" +
                        $"**Length** ``{response.Tracks.Sum(x => x.Duration.TotalSeconds):m\\:ss}``");
                }
                else
                {
                    var track = response.Tracks[0];
                    player.Queue.Enqueue(track);
                    users.Enqueue(state as SocketGuildUser);

                    await reply.MessageAsync(channel, 
                        $"**Enqueued** ``{track.Title}``\n" +
                        $"**Length** ``{track.Duration:m\\:ss}``");
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

                    var duration = new TimeSpan(0, 0, response.Tracks.Sum(x => x.Duration.Seconds));
                    await reply.MessageAsync(channel,
                        $"**Enqueued** ``{response.Tracks.Count} tracks``\n" +
                        $"**Length** ``{duration:m\\:ss}``");
                }
                else
                {
                    await player.PlayAsync(track);
                    users.Enqueue(state as SocketGuildUser);
                }
            }
        }

        public async Task DurationAsync()
        {
            if (player is null)
            {
                await reply.ErrorAsync(channel, $"I'm not connected to any voice-channel");
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

        public async Task SkipAsync(int amount)
        {
            if (player is null)
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
            {
                await reply.MessageAsync(channel,
                    $"**Skipped** ``{track.Title}``\n" +
                    $"**Tracks in Queue** ``{player.Queue.Count}``");
            }
            else
            {
                var content = $"**Skipped** ``{track.Title}``\n";
                for (int i = 0; i < amount - 1; i++)
                {
                    if (player.Queue.TryDequeue(out track))
                    {
                        content += $"**Skipped** ``{track.Title}``\n";
                    }
                    else
                    {
                        content += $"**Failed to Skip** ``{track.Title}``\n";
                    }
                }
                content += $"\n**Tracks in Queue** ``{player.Queue.Count}``";
                await reply.MessageAsync(channel, content);
            }

            await player.StopAsync();
        }

        public async Task CurrentAsync()
        {
            if (player is null)
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
            await reply.MessageAsync(channel, 
                $"**Playing** ``{track.Title}``\n" +
                $"**Length** ``{track.Duration:m\\:ss}``\n" +
                $"**Requested By** ``{users.Peek().Username}``\n" +
                $"**Volume** ``{player.Volume}``\n" +
                $"**Repeat** ``{repeat}``");
        }

        public async Task ArtworkAsync()
        {
            if (player is null)
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

            await reply.ErrorAsync(channel, "This command is currently not working");
        }

        public async Task RepeatAsync()
        {
            if (player is null)
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }

            repeat = !repeat;
            string result = repeat is true ? "Repeat turned on" : "Repeat turned off";
            await reply.MessageAsync(channel, result);
        }

        public async Task VolumeAsync(ushort volume)
        {
            if (player is null)
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
                await reply.ErrorAsync(channel, "No more tracks in the queue");
                await lavaNode.LeaveAsync(args.Player.VoiceChannel);
                channel = null;
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

            await reply.MessageAsync(channel, 
                $"**Playing** ``{args.Track.Title}``\n" +
                $"**Length** ``{args.Track.Duration:m\\:ss}``");
        }

        private async Task<SearchResponse> SearchAsync(string query)
        {
            var response = await lavaNode.SearchAsync(query);
            if (response.Tracks.Count > 0)
            {
                return response;
            }

            return await lavaNode.SearchYouTubeAsync(query);
        }
    }
}
