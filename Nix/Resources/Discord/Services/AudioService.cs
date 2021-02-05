using Discord;
using Discord.WebSocket;
using SpotifyAPI.Web;
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
        private readonly int length = 40;
        private ITextChannel channel;
        private Queue<SocketGuildUser> users = new Queue<SocketGuildUser>();
        private bool repeat = false;
        private LavaPlayer player;
        private SpotifyClient spotify;

        public AudioService(LavaNode lavaNode, EmbedService reply, ScriptService script)
        {
            Task.Run(async () => await script.RunScript("run_lavalink.ps1"));
            this.lavaNode = lavaNode;
            this.reply = reply;
            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStarted += OnTrackStart;

            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(
                    Config.Data.SpotifyId,
                    Config.Data.SpotifySecret));
            spotify = new SpotifyClient(config);
        }

        public async Task<bool> JoinAsync(IVoiceState state, ITextChannel channel, bool command = false)
        {
            if (!(player is null))
            {
                if (command)
                {
                    return true;
                }
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

        public async Task<bool> LeaveAsync(IVoiceState state, ITextChannel channel)
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
                this.channel = null;
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
            if (!await JoinAsync(state, channel, true))
                return;

            var response = await SearchAsync(search);
            if (response.LoadStatus == LoadStatus.LoadFailed ||
                response.LoadStatus == LoadStatus.NoMatches)
            {
                if (await PlaySpotifyAsync(state, channel, search))
                    return;

                await reply.ErrorAsync(channel, "No matches");
                return;
            }

            var player = lavaNode.GetPlayer(state?.VoiceChannel.Guild);
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

                    var duration = new TimeSpan(0, 0, response.Tracks.Sum(x => x.Duration.Seconds));
                    await reply.MessageAsync(channel,
                        $"**Enqueued** ``{response.Tracks.Count} tracks``\n" +
                        $"**Length** ``{duration:m\\:ss}``");
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

        public async Task<bool> PlaySpotifyAsync(IVoiceState state, ITextChannel channel, string url)
        {
            string id;
            FullPlaylist playlist;
            try
            {
                id = url.Substring(34);
                id = id.Substring(0, id.IndexOf("?"));
                playlist = await spotify.Playlists.Get(id);
            }
            catch
            {
                return false;
            }

            if (string.IsNullOrEmpty(id))
                return false;
            if (!await JoinAsync(state, channel, true))
                return false;

            await reply.MessageAsync(channel,
                    $"**Playlist** [{playlist.Name}]({url})\n" +
                    $"**Enqueued** {playlist.Tracks.Items.Count} tracks");

            for (int i = 0; i < playlist.Tracks.Items.Count; i++)
            {
                var item = playlist.Tracks.Items[i];
                var response = await lavaNode.SearchYouTubeAsync((item.Track as FullTrack).Name);

                if (response.LoadStatus == LoadStatus.LoadFailed ||
                    response.LoadStatus == LoadStatus.NoMatches)
                {
                    continue;
                }
                if (player.PlayerState == PlayerState.Stopped ||
                    player.PlayerState == PlayerState.Connected)
                {
                    await player.PlayAsync(response.Tracks[0]);
                    users.Enqueue(state as SocketGuildUser);
                    continue;
                }

                player.Queue.Enqueue(response.Tracks[0]);
                users.Enqueue(state as SocketGuildUser);
            }
            return true;
        }

        public async Task DurationAsync(ITextChannel channel)
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

        public async Task SkipAsync(ITextChannel channel, int amount)
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
                    $"**Skipped** {GetTitleAsUrl(track)}\n" +
                    $"**Tracks in Queue** ``{player.Queue.Count}``");
            }
            else
            {
                int skipped = 0;
                int failed = 0;
                for (int i = 0; i < amount - 1; i++)
                {
                    if (player.Queue.TryDequeue(out _))
                        skipped++;
                    else failed++;
                }
                var content = $"**Skipped** ``{skipped} tracks``\n" +
                    $"**Failed to Skip** ``{failed} tracks``";
                content += $"\n**Tracks in Queue** ``{player.Queue.Count}``";
                await reply.MessageAsync(channel, content);
            }

            await player.StopAsync();
        }

        public async Task CurrentAsync(ITextChannel channel)
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
                $"**Playing** {GetTitleAsUrl(track)}\n" +
                $"**Length** ``{track.Duration:m\\:ss}``\n" +
                $"**Requested By** ``{users.Peek().Username}``\n" +
                $"**Volume** ``{player.Volume}``\n" +
                $"**Repeat** ``{repeat}``");
        }

        public async Task ArtworkAsync(ITextChannel channel)
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
            if (player is null)
            {
                await context.Channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (player.Queue.Count <= 0)
            {
                await reply.ErrorAsync(context.Channel as ITextChannel, "No more tracks in the queue");
                return;
            }

            var content = "";
            var tracks = player.Queue.ToList();
            var pages = new List<string>();

            for (int i = 0; i < tracks.Count; )
            {
                var track = tracks[i];
                content += $"**{++i}** {GetTitleAsUrl(track)} **|** ``{track.Duration:m\\:ss}``\n";
                if (i % 10 == 0)
                {
                    pages.Add(content);
                    content = "";
                }
            }

            if (!string.IsNullOrEmpty(content))
            {
                pages.Add(content);
            }

            await reply.PaginatedMessageAsync(context, pages: pages);
        }

        public async Task RepeatAsync(ITextChannel channel)
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

        public async Task VolumeAsync(ITextChannel channel, ushort volume)
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

        public async Task ShuffleAsync(ITextChannel channel)
        {
            if (player is null)
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (player.Queue.Count <= 0)
            {
                await reply.ErrorAsync(channel, "There are no tracks in the queue");
                return;
            }

            player.Queue.Shuffle();
            await reply.MessageAsync(channel, "Playlist has been shuffled");
        }

        public async Task SeekAsync(ITextChannel channel, ushort seconds)
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

            var time = TimeSpan.FromSeconds(seconds);
            time = time >= player.Track.Duration ? player.Track.Duration : time;
            await player.SeekAsync(time);
        }

        public async Task PauseAsync(ITextChannel channel)
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

            await player.PauseAsync();
            await reply.MessageAsync(channel, "Player was paused\n" +
                "Resume using ``.resume``");
        }

        public async Task ResumeAsync(ITextChannel channel)
        {
            if (player is null)
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (player.PlayerState != PlayerState.Paused)
            {
                await reply.ErrorAsync(channel, "Player is not paused");
                return;
            }

            await player.ResumeAsync();
            await reply.MessageAsync(channel, "Resumed playing again");
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
                this.player = null;
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
                $"**Playing** {GetTitleAsUrl(args.Track)}\n" +
                $"**Length** ``{args.Track.Duration:m\\:ss}``");
        }

        private async Task<Victoria.Responses.Rest.SearchResponse> SearchAsync(string query)
        {
            var response = await lavaNode.SearchAsync(query);
            if (response.Tracks.Count > 0)
            {
                return response;
            }

            return await lavaNode.SearchYouTubeAsync(query);
        }

        private string FormatTitleLength(string title)
            => title.Length > length ? title.Substring(0, length) + "..." : title;

        private string GetTitleAsUrl(LavaTrack track)
            => $"[{FormatTitleLength(track.Title)}]({track.Url})";
    }
}
