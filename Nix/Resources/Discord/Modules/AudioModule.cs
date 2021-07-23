using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    [RequireVoice]
    [AudioBindChannel]
    public class AudioModule : ModuleBase<NixCommandContext>
    {
        public AudioService Audio { get; set; }
        public NixPlayer Player
        {
            get
            {
                if (!Audio.TryGetPlayer(Context.Guild, out NixPlayer player))
                    return null;
                return player;
            }
        }

        [Command("join")]
        public async Task JoinAsync()
        {
            var player = await Audio.CreatePlayerForGuildAsync(
                Context.Guild, 
                Context.VoiceChannel, 
                Context.TextChannel);

            if (player is null)
            {
                await Context.TextChannel.SendMessageAsync("Oh no, something happened...");
                return;
            }

            await Context.TextChannel.SendMessageAsync(
                $"Joined **{player.VoiceChannel.Name}** and bound to **{player.TextChannel.Name}**");
        }

        [Command("leave")]
        [EnsurePlayerConnection]
        public async Task LeaveAsync()
        {
            await Player.TextChannel.SendMessageAsync($"Left **{Player.VoiceChannel.Name}**");
            await Player.LeaveAsync();
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder] string search)
        {
            if (Player is null)
            {
                await JoinAsync();
            }

            var tracks = await Player.PlayAsync(search);
            await Player.TextChannel.SendMessageAsync($"**Enqueued** ``{tracks.Length}`` tracks");
        }

        [Command("duration")]
        [EnsurePlayerConnection]
        public async Task DurationAsync()
        {
            await Player.TextChannel.SendMessageAsync(
                $"**{Player.CurrentTrack.Position:m\\:ss}** / " +
                $"**{Player.CurrentTrack.Duration:m\\:ss}**");
        }

        [Command("skip")]
        [EnsurePlayerConnection]
        public async Task SkipAsync(int amount = 1)
        {
            int skipped = await Player.SkipAsync(amount);
            await Player.TextChannel.SendMessageAsync($"**Skipped** ``{skipped}`` tracks");
        }

        [Command("current")]
        [EnsurePlayerConnection]
        public async Task CurrentAsync()
        {
            await Player.TextChannel.SendMessageAsync($"**Now Playing** {Player.CurrentTrack.Title}");
        }

        [Command("artwork")]
        [EnsurePlayerConnection]
        public async Task ArtworkAsync()
        {
            await Player.TextChannel.SendMessageAsync(await Player.GetArtworkAsync());
        }

        [Command("queue")]
        [EnsurePlayerConnection]
        public async Task QueueAsync()
        {
            var content = "";
            var tracks = Player.Queue;
            var pages = new List<string>();

            for (int i = 0; i < tracks.Length;)
            {
                var track = tracks[i];
                content += $"**{++i}** {await Player.GetTitleAsUrlAsync(track)} **|** ``{track.Duration:m\\:ss}``\n";
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

            await Context.Reply.PaginatedMessageAsync(Context, pages);
        }

        [Command("repeat")]
        [EnsurePlayerConnection]
        public async Task RepeatAsync()
        {
            Player.OnRepeat = !Player.OnRepeat;
            await Player.TextChannel.SendMessageAsync($"**Looping** {Player.OnRepeat}");
        }

        [Command("volume")]
        [EnsurePlayerConnection]
        public async Task VolumeAsync(ushort volume)
        {
            int previous = Player.Volume;
            await Player.SetVolumeAsync(volume);
            await Player.TextChannel.SendMessageAsync($"**Changed Volume** {previous} -> {Player.Volume}");
        }

        [Command("shuffle")]
        [EnsurePlayerConnection]
        public async Task ShuffleAsync()
        {
            await Player.Shuffle();
            await Player.TextChannel.SendMessageAsync("**Playlist shuffled**");
        }

        [Command("time")]
        [EnsurePlayerConnection]
        public async Task TimeAsync(int seconds)
        {
            await Player.SeekAsync(TimeSpan.FromSeconds(seconds));
            await Player.TextChannel.SendMessageAsync($"**Position** {Player.CurrentTrack.Position:m\\:ss}");
        }

        [Command("pause")]
        [EnsurePlayerConnection]
        public async Task PauseAsync()
        {
            if (!await Player.PauseAsync())
            {
                await Player.TextChannel.SendMessageAsync("Player is already paused, use ``resume`` to continue playing...");
                return;
            }

            await Player.TextChannel.SendMessageAsync("**Paused**");
        }

        [Command("resume")]
        [EnsurePlayerConnection]
        public async Task ResumeAsync()
        {
            if (!await Player.ResumeAsync())
            {
                await Player.TextChannel.SendMessageAsync("Player is already playing, use ``pause`` to pause...");
                return;
            }

            await Player.TextChannel.SendMessageAsync("**Resumed Playing**");
        }

        [Command("lyrics")]
        [EnsurePlayerConnection]
        public async Task LyricsAsync()
        {
            await Player.TextChannel.SendMessageAsync(await Player.GetLyricsAsync());
        }
    }
}
