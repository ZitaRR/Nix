using Discord;
using Discord.WebSocket;
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

        public EmbedFooterBuilder Footer
            => new EmbedFooterBuilder
            {
                Text = $"{DateTime.UtcNow:yyyy\\-mm\\-dd} - {DateTime.UtcNow:HH\\:mm} UTC ◈ Latency: {client.Latency}ms",
                IconUrl = client.CurrentUser.GetAvatarUrl()
            };

        public ReplyService(DiscordSocketClient client)
        {
            this.client = client;
            color = new Color(254, 254, 254);
            offset = 25;
        }

        private string FormatTrackTitle(string title)
            => title.Length > offset ? title.Substring(0, offset) + "..." : title;

        public async Task AudioPlayAsync(ITextChannel channel, SocketGuildUser user, LavaTrack track)
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
    }
}
