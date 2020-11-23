using Discord;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Victoria;

namespace Nix.Resources.Discord
{
    public sealed class ReplyService
    {
        private readonly Color NormalColor;
        private readonly Color errorColor;
        private readonly int length;
        private EmbedBuilder embed;
        private DiscordSocketClient client;
        private InteractiveService interactive;

        public EmbedFooterBuilder Footer
            => new EmbedFooterBuilder
            {
                Text = $"{DateTime.UtcNow:yyyy\\-MM\\-dd} - {DateTime.UtcNow:HH\\:mm} UTC ◈ Latency: {client.Latency}ms",
                IconUrl = client.CurrentUser.GetAvatarUrl()
            };

        public ReplyService(DiscordSocketClient client, InteractiveService interactive)
        {
            this.client = client;
            this.interactive = interactive;
            NormalColor = new Color(254, 254, 254);
            errorColor = new Color(254, 50, 50);
            length = 25;
        }

        private string FormatTrackTitle(string title)
            => title.Length > length ? title.Substring(0, length) + "..." : title;

        public async Task AudioPlayAsync(ITextChannel channel, LavaTrack track)
        {
            var title = FormatTrackTitle(track.Title);
            embed = new EmbedBuilder
            {
                Description =   $"**Playing** 🎶 ``{title}`` 🎶\n" +
                                $"**Length** ⌚ ``{track.Duration:m\\:ss}``",
                Color = NormalColor,
                Footer = Footer
            };
            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioEnqueueSingleAsync(ITextChannel channel, LavaTrack track)
        {
            var title = FormatTrackTitle(track.Title);
            embed = new EmbedBuilder
            {
                Description =   $"**Enqueued** 🎶 ``{title}`` 🎶\n" +
                                $"**Length** ⌚ ``{track.Duration:mm\\:ss}``",
                Color = NormalColor,
                Footer = Footer
            };
            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioEnqueueManyAsync(ITextChannel channel, IReadOnlyCollection<LavaTrack> tracks)
        {
            embed = new EmbedBuilder
            {
                Description =   $"**Enqueued** ``{tracks.Count} tracks``\n" +
                                $"**Length** ``{tracks.Sum(x => x.Duration.TotalSeconds)}s``",
                Color = NormalColor,
                Footer = Footer
            };
            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioCurrentlyPlayingAsync(ITextChannel channel, SocketGuildUser user,
            LavaTrack track, int volume, bool repeat)
        {
            var title = FormatTrackTitle(track.Title);
            var onRepeat = repeat is true ? "On" : "Off";
            embed = new EmbedBuilder
            {
                Description =   $"**Playing** 🎶 ``{title}`` 🎶\n" +
                                $"**Length** ⌚ ``{track.Position:m\\:ss} / {track.Duration:m\\:ss}``\n" +
                                $"**Requested By** ``{user.Username}``\n" +
                                $"**Volume** ``{volume}``\n" +
                                $"**Repeat** ``{onRepeat}``",
                Color = NormalColor,
                Footer = Footer
            };
            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioSkipAsync(ITextChannel channel, LavaTrack track, int amount)
        {
            var title = FormatTrackTitle(track.Title);
            embed = new EmbedBuilder
            {
                Description =   $"**Skipped** :x: ``{title}`` :x:\n" +
                                $"**Tracks in Queue** ``{amount}``",
                Color = NormalColor,
                Footer = Footer
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioSkipAsync(ITextChannel channel, int skipped, int amount)
        {
            embed = new EmbedBuilder
            {
                Description =   $"**Skipped** :x: ``{skipped} tracks`` :x:\n" +
                                $"**Tracks in Queue** ``{amount}``",
                Color = NormalColor,
                Footer = Footer
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioQueueAsync(NixCommandContext context, List<LavaTrack> tracks, LavaTrack track)
        {
            var content = "";
            var pages = new List<string>();
            for (int i = 0; i < tracks.Count; i++)
            {
                var _track = tracks[i];
                content += $"**{i + 1}** ``{FormatTrackTitle(_track.Title)}`` **|** ``{_track.Duration:m\\:ss}``\n";
                if (i != 0 && i % 9 == 0)
                {
                    pages.Add(content);
                    content = "";
                }
            }

            if (!string.IsNullOrEmpty(content))
                pages.Add(content);

            var title = FormatTrackTitle(track.Title);
            var pager = new PaginatedMessage
            {
                Title = $"**Playing** 🎶 ``{title}`` 🎶",
                Pages = pages,
                Color = NormalColor,
                AlternateDescription = null
            };

            await interactive.SendPaginatedMessageAsync(context, pager);
        }

        public async Task ErrorAsync(ITextChannel channel, string message)
        {
            embed = new EmbedBuilder
            {
                Description = message,
                Color = errorColor,
                Footer = Footer
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task ExceptionAsync(ITextChannel channel, Exception e)
        {
            embed = new EmbedBuilder
            {
                Description = $"**ERROR** ``{e.HResult}``\n{e.StackTrace}",
                Color = errorColor,
                Footer = Footer
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task MessageAsync(ITextChannel channel, string message)
        {
            embed = new EmbedBuilder
            {
                Description = message,
                Color = NormalColor,
                Footer = Footer
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
