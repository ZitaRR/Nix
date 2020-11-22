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
        private readonly Color color;
        private readonly int offset;
        private EmbedBuilder embed;
        private DiscordSocketClient client;
        private InteractiveService interactive;

        public EmbedFooterBuilder Footer
            => new EmbedFooterBuilder
            {
                Text = $"{DateTime.UtcNow:yyyy\\-mm\\-dd} - {DateTime.UtcNow:HH\\:mm} UTC ◈ Latency: {client.Latency}ms",
                IconUrl = client.CurrentUser.GetAvatarUrl()
            };

        public ReplyService(DiscordSocketClient client, InteractiveService interactive)
        {
            this.client = client;
            this.interactive = interactive;
            color = new Color(254, 254, 254);
            offset = 25;
        }

        private string FormatTrackTitle(string title)
            => title.Length > offset ? title.Substring(0, offset) + "..." : title;

        public async Task AudioPlayAsync(ITextChannel channel, LavaTrack track)
        {
            var title = FormatTrackTitle(track.Title);
            embed = new EmbedBuilder
            {
                Description =   $"**Playing** 🎶 ``{title}`` 🎶\n" +
                                $"**Length** ⌚ ``{track.Duration:m\\:ss}``",
                Color = color,
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
                Color = color,
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
                Color = color,
                Footer = Footer
            };
            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task AudioCurrentlyPlayingAsync(ITextChannel channel, SocketGuildUser user, LavaTrack track)
        {
            var title = FormatTrackTitle(track.Title);
            embed = new EmbedBuilder
            {
                Description =   $"**Playing** 🎶 ``{title}`` 🎶\n" +
                                $"**Length** ⌚ ``{track.Position:m\\:ss} / {track.Duration:m\\:ss}``",
                Color = color,
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
                Color = color,
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
                Color = color,
                AlternateDescription = null
            };

            await interactive.SendPaginatedMessageAsync(context, pager);
        }
    }
}
