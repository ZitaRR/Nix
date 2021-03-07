using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;
using Victoria.Responses.Rest;
using Nix.Models;

namespace Nix.Resources.Discord
{
    public sealed class AudioService
    {
        private readonly LavaNode lavaNode;
        private readonly ILogger logger;
        private readonly EmbedService reply;
        private readonly SpotifyService spotify;
        private readonly IDiscord discord;
        private readonly TimeSpan inactivity = TimeSpan.FromMinutes(1);
        private readonly ushort defaultVolume = 50;
        private readonly int length = 40;
        private ConcurrentDictionary<ulong, NixPlayer> players;
        private ConcurrentDictionary<LavaPlayer, LavalinkData> data;

        public AudioService(
            LavaNode lavaNode,
            ILogger logger,
            EmbedService reply,
            ScriptService script,
            SpotifyService spotify,
            IDiscord discord)
        {
            Task.Run(async () => await script.RunScript("run_lavalink.ps1"));
            this.lavaNode = lavaNode;
            this.logger = logger;
            this.reply = reply;
            this.spotify = spotify;
            this.discord = discord;

            this.lavaNode.OnTrackEnded += OnTrackEnd;
            this.lavaNode.OnTrackStarted += OnTrackStart;
            this.lavaNode.OnTrackStuck += OnTrackStuck;
            this.lavaNode.OnTrackException += OnTrackException;
            this.discord.Client.Disconnected += OnDisconnection;
            this.discord.Client.Ready += OnReady;

            players = new ConcurrentDictionary<ulong, NixPlayer>();
            data = new ConcurrentDictionary<LavaPlayer, LavalinkData>();
        }

        public async Task<bool> JoinAsync(IVoiceState state, ITextChannel channel, bool command = false)
        {
            if (players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                if (command)
                {
                    return true;
                }
                await reply.ErrorAsync(channel, $"I'm already connected to {nix.VoiceChannel.Name}");
                return true;
            }

            try
            {
                await lavaNode.JoinAsync(state.VoiceChannel, channel)
                    .ContinueWith(async (p) =>
                    {
                        var nix = new NixPlayer(await p);
                        players.TryAdd(channel.GuildId, nix);
                        await nix.Player.UpdateVolumeAsync(defaultVolume);
                    });
                logger.AppendLog("AUDIO", $"New player in [{channel.Guild.Name}]. " +
                    $"({players.Count - 1} -> {players.Count})");
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
            if (!players.TryGetValue(channel.GuildId, out _))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return true;
            }

            try
            {
                await lavaNode.LeaveAsync(state?.VoiceChannel);
                players.Remove(channel.GuildId, out NixPlayer nix);
                logger.AppendLog("AUDIO", $"Player disposed in [{channel.Guild.Name}]. " +
                    $"({players.Count + 1} -> {players.Count})");
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
            if (spotify.IsSpotifyUri(search))
            {
                await PlaySpotifyAsync(state, channel, search);
                return;
            }

            var response = await SearchAsync(search);
            if (response.LoadStatus == LoadStatus.LoadFailed ||
                response.LoadStatus == LoadStatus.NoMatches)
            {
                await reply.ErrorAsync(channel, "No matches");
                return;
            }


            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "Could not find the player!");
                return;
            }

            if (nix.Player.PlayerState == PlayerState.Playing ||
                nix.Player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(response.Playlist.Name))
                {
                    foreach (var track in response.Tracks)
                    {
                        nix.Player.Queue.Enqueue(track);
                    }

                    var duration = new TimeSpan(0, 0, response.Tracks.Sum(x => x.Duration.Seconds));
                    await reply.MessageAsync(channel,
                        $"**Enqueued** ``{response.Tracks.Count} tracks``\n" +
                        $"**Length** ``{duration:m\\:ss}``");
                }
                else
                {
                    var track = response.Tracks[0];
                    nix.Player.Queue.Enqueue(track);

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
                            await nix.Player.PlayAsync(track);
                        else nix.Player.Queue.Enqueue(response.Tracks[i]);
                    }

                    var duration = new TimeSpan(0, 0, response.Tracks.Sum(x => x.Duration.Seconds));
                    await reply.MessageAsync(channel,
                        $"**Enqueued** ``{response.Tracks.Count} tracks``\n" +
                        $"**Length** ``{duration:m\\:ss}``");
                }
                else
                {
                    await nix.Player.PlayAsync(track);
                }
            }
        }

        public async Task PlaySpotifyAsync(IVoiceState state, ITextChannel channel, string url)
        {
            if (!await JoinAsync(state, channel, true))
                return;
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
                return;

            SpotifyAPI.Web.FullTrack track;

            if (spotify.IsPlaylist(url))
            {
                var playlist = await spotify.GetPlaylist(url);

                if (playlist != null)
                {
                    await reply.MessageAsync(channel,
                        $"**Playlist** [{playlist.Value.playlistName}]({url})\n" +
                        $"**Enqueued** {playlist.Value.tracks.Count} tracks");

                    for (int i = 0; i < playlist.Value.tracks.Count; i++)
                    {
                        track = playlist.Value.tracks[i];
                        var response = await lavaNode.SearchYouTubeAsync(
                            $"{track.Artists[0].Name} {track.Name} Audio");

                        if (response.LoadStatus == LoadStatus.LoadFailed ||
                            response.LoadStatus == LoadStatus.NoMatches)
                        {
                            continue;
                        }
                        if (nix.Player.PlayerState == PlayerState.Stopped ||
                            nix.Player.PlayerState == PlayerState.Connected)
                        {
                            await nix.Player.PlayAsync(response.Tracks[0]);
                            continue;
                        }

                        nix.Player.Queue.Enqueue(response.Tracks[0]);
                    }
                    return;
                }
            }
            else
            {
                track = await spotify.GetTrack(url);
                if (track != null)
                {
                    var response = await lavaNode.SearchYouTubeAsync(
                        $"{track.Artists[0].Name} {track.Name} Audio");

                    if (response.LoadStatus == LoadStatus.LoadFailed ||
                        response.LoadStatus == LoadStatus.NoMatches)
                        return;
                    if (nix.Player.PlayerState == PlayerState.Stopped ||
                        nix.Player.PlayerState == PlayerState.Connected)
                    {
                        await nix.Player.PlayAsync(response.Tracks[0]);
                        return;
                    }

                    nix.Player.Queue.Enqueue(response.Tracks[0]);
                    return;
                }
            }

            return;
        }

        public async Task DurationAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to any voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            LavaTrack track = nix.Player.Track;
            await reply.MessageAsync(channel, $"{track.Position:mm\\:ss} / {track.Duration:mm\\:ss}");
        }

        public async Task SkipAsync(ITextChannel channel, int amount)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            var track = nix.Player.Track;
            if (amount == 1)
            {
                await reply.MessageAsync(channel,
                    $"**Skipped** {GetTitleAsUrl(track)}\n" +
                    $"**Tracks in Queue** ``{nix.Player.Queue.Count}``");
            }
            else
            {
                int skipped = 0;
                int failed = 0;
                for (int i = 0; i < amount - 1; i++)
                {
                    if (nix.Player.Queue.TryDequeue(out _))
                        skipped++;
                    else failed++;
                }
                var content = $"**Skipped** ``{skipped} tracks``\n" +
                    $"**Failed to Skip** ``{failed} tracks``";
                content += $"\n**Tracks in Queue** ``{nix.Player.Queue.Count}``";
                await reply.MessageAsync(channel, content);
            }

            await nix.Player.StopAsync();
        }

        public async Task CurrentAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped || 
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            LavaTrack track = nix.Player.Track;
            await reply.MessageAsync(channel, 
                $"**Playing** {GetTitleAsUrl(track)}\n" +
                $"**Length** ``{track.Duration:m\\:ss}``\n" +
                $"**Volume** ``{nix.Player.Volume}``\n" +
                $"**Repeat** ``{nix.OnRepeat}``");
        }

        public async Task ArtworkAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState != PlayerState.Playing ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            string artwork = await nix.Player.Track.FetchArtworkAsync();
            await channel.SendMessageAsync(artwork);
        }

        public async Task ListQueueAsync(NixCommandContext context)
        {
            if (!players.TryGetValue(context.Guild.Id, out NixPlayer nix))
            {
                await context.Channel.SendMessageAsync("I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.Queue.Count <= 0)
            {
                await reply.ErrorAsync(context.Channel as ITextChannel, "No more tracks in the queue");
                return;
            }

            var content = "";
            var tracks = nix.Player.Queue.ToList();
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

            await reply.PaginatedMessageAsync(context, pages);
        }

        public async Task RepeatAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, $"I'm not connected to a voice-channel");
                return;
            }

            nix.OnRepeat = !nix.OnRepeat;
            string result = nix.OnRepeat is true ? "Repeat turned on" : "Repeat turned off";
            await reply.MessageAsync(channel, result);
        }

        public async Task VolumeAsync(ITextChannel channel, ushort volume)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            var previous = nix.Player.Volume;
            await nix.Player.UpdateVolumeAsync(volume);
            await reply.MessageAsync(channel, $"Changed volume to {nix.Player.Volume} from {previous}");
        }

        public async Task ShuffleAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.Queue.Count <= 0)
            {
                await reply.ErrorAsync(channel, "There are no tracks in the queue");
                return;
            }

            nix.Player.Queue.Shuffle();
            await reply.MessageAsync(channel, "Playlist has been shuffled");
        }

        public async Task SeekAsync(ITextChannel channel, ushort seconds)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            var time = TimeSpan.FromSeconds(seconds);
            time = time >= nix.Player.Track.Duration ? nix.Player.Track.Duration : time;
            await nix.Player.SeekAsync(time);
        }

        public async Task PauseAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped ||
                nix.Player.Track is null)
            {
                await reply.ErrorAsync(channel, "Nothing is currently playing");
                return;
            }

            await nix.Player.PauseAsync();
            await reply.MessageAsync(channel, "Player was paused\n" +
                "Resume using ``.resume``");
        }

        public async Task ResumeAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState != PlayerState.Paused)
            {
                await reply.ErrorAsync(channel, "Player is not paused");
                return;
            }

            await nix.Player.ResumeAsync();
            await reply.MessageAsync(channel, "Resumed playing again");
        }

        public async Task LyricsAsync(ITextChannel channel)
        {
            if (!players.TryGetValue(channel.GuildId, out NixPlayer nix))
            {
                await reply.ErrorAsync(channel, "I'm not connected to a voice-channel");
                return;
            }
            if (nix.Player.PlayerState == PlayerState.Stopped)
            {
                await reply.MessageAsync(channel, "Nothing is currently playing");
                return;
            }

            var lyrics = await nix.Player.Track.FetchLyricsFromGeniusAsync();
            if (string.IsNullOrEmpty(lyrics))
                lyrics = await nix.Player.Track.FetchLyricsFromOVHAsync();
            if (string.IsNullOrEmpty(lyrics))
                lyrics = "Could not find any lyrics for the current playing track";

            await reply.MessageAsync(channel, lyrics);
        }

        public async Task InitiateDisconnectAsync(IGuild guild)
        {
            if (!players.TryGetValue(guild.Id, out NixPlayer nix))
                return;

            var cancelled = SpinWait.SpinUntil(() => nix.Token.IsCancellationRequested, inactivity);

            if (cancelled)
            {
                nix.ResetCancellation();
                return;
            }

            await reply.MessageAsync(nix.TextChannel, "Leaving due to inactivity");
            await lavaNode.LeaveAsync(nix.VoiceChannel);
            players.Remove(guild.Id, out _);
            logger.AppendLog("AUDIO", $"Player disposed in [{guild.Name}]." +
                $"({players.Count + 1} -> {players.Count})");
        }

        public Task CancelDisconnect(IGuild guild)
        {
            if (!players.TryGetValue(guild.Id, out NixPlayer nix))
                return Task.CompletedTask;

            nix.Source.Cancel();
            return Task.CompletedTask;
        }

        private async Task OnTrackEnd(TrackEndedEventArgs args)
        {
            IGuild guild = args.Player.VoiceChannel.Guild;
            if (!players.TryGetValue(guild.Id, out NixPlayer nix))
                return;

            if (nix.OnRepeat)
            {
                await nix.Player.PlayAsync(args.Track);
                return;
            }

            if (!nix.Player.Queue.TryDequeue(out var track))
            {
                await reply.ErrorAsync(nix.TextChannel, "No more tracks in the queue\n" +
                    $"Leaving in {inactivity:m\\:ss}");
                await InitiateDisconnectAsync(guild);
                return;
            }

            await nix.Player.PlayAsync(track);
        }

        private async Task OnTrackStart(TrackStartEventArgs args)
        {
            if (args.Player.PlayerState != PlayerState.Playing ||
                args.Track is null)
                return;

            await reply.MessageAsync(args.Player.TextChannel, 
                $"**Playing** {GetTitleAsUrl(args.Track)}\n" +
                $"**Length** ``{args.Track.Duration:m\\:ss}``");
        }

        private Task OnTrackStuck(TrackStuckEventArgs args)
        {
            //Placeholder
            logger.AppendLog("AUDIO", $"{args.Track.Title} got stuck");
            return Task.CompletedTask;
        }

        private Task OnTrackException(TrackExceptionEventArgs args)
        {
            //Placeholder
            logger.AppendLog("AUDIO", $"{args.Track.Title} threw an exception\n" +
                $"{args.ErrorMessage}");
            return Task.CompletedTask;
        }

        private async Task OnDisconnection(Exception e)
        {
            foreach (NixPlayer nix in players.Values)
            {
                data.TryAdd(nix.Player, new LavalinkData(nix));
                await lavaNode.LeaveAsync(nix.VoiceChannel);
            }

            players.Clear();
        }

        private async Task OnReady()
        {
            foreach (var data in data)
            {
                await lavaNode.JoinAsync(data.Value.VoiceChannel, data.Value.TextChannel)
                    .ContinueWith(async (p) =>
                    {
                        var nix = new NixPlayer(await p);
                        players.TryAdd(nix.VoiceChannel.GuildId, nix);

                        await nix.Player.UpdateVolumeAsync((ushort)data.Value.Volume);
                        await nix.Player.PlayAsync(data.Value.CurrentTrack);
                        await nix.Player.SeekAsync(data.Value.Position);

                        foreach (var track in data.Value.Queue)
                        {
                            nix.Player.Queue.Enqueue(track);
                        }
                    });
            }

            data.Clear();
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

        private string FormatTitleLength(string title)
            => title.Length > length ? title.Substring(0, length) + "..." : title;

        private string GetTitleAsUrl(LavaTrack track)
            => $"[{FormatTitleLength(track.Title)}]({track.Url})";

        public NixPlayer GetPlayer(ulong id)
        {
            if (!players.TryGetValue(id, out NixPlayer nix))
                return null;
            return nix;
        }
    }
}
