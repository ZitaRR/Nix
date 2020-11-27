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
    public sealed class EmbedService
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

        public EmbedService(DiscordSocketClient client, InteractiveService interactive)
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

        public async Task EventAsync(ITextChannel channel, SocketUser creator,
            string name, string description, int id, DateTime start)
        {
            embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = creator.Username,
                    IconUrl = creator.GetAvatarUrl()
                },
                Title = name,
                Description =   $"**ID** ``{id}``\n" +
                                $"**Time** ``{start:yyyy-MM-dd, HH:mm UTCz}``\n\n" +
                                $"{description}",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Participants",
                        Value = "N/A"
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Interested",
                        Value = "N/A"
                    }
                },
                Color = NormalColor,
                Footer = Footer
            };

            var message = await channel.SendMessageAsync(embed: embed.Build());
            await message.AddReactionAsync(new Emoji("✔️"));
            await message.AddReactionAsync(new Emoji("❌"));
            await message.AddReactionAsync(new Emoji("❔"));
        }

        public async Task EventUpdateAsync(ITextChannel channel, SocketReaction react, NixEvent nixEvent)
        {
            var message = await channel.GetMessageAsync(nixEvent.MessageID) as IUserMessage;
            if (message is null)
                return;

            var participants = nixEvent.Participants.FirstOrDefault()?.Name ?? "N/A";
            for (int i = 1; i < nixEvent.Participants.Count; i++)
            {
                participants += $", {nixEvent.Participants[i].Name}";
            }

            var interested = nixEvent.PossibleParticipants.FirstOrDefault()?.Name ?? "N/A";
            for (int i = 1; i < nixEvent.PossibleParticipants.Count; i++)
            {
                participants += $", {nixEvent.PossibleParticipants[i].Name}";
            }

            embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = nixEvent.Creator.Name,
                    IconUrl = nixEvent.Creator.AvatarURL
                },
                Title = nixEvent.Name,
                Description =   $"**ID** ``{nixEvent.ID}``\n" +
                                $"**Time** ``{nixEvent.Start:yyyy-MM-dd, HH:mm UTCz}``\n\n" +
                                $"{nixEvent.Description}",
                Fields = new List<EmbedFieldBuilder> 
                { 
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Participants",
                        Value = participants
                    },
                    new EmbedFieldBuilder
                    {
                        IsInline = true,
                        Name = "Interested",
                        Value = interested
                    }
                },
                Color = NormalColor,
                Footer = Footer
            };

            await message.ModifyAsync(x => x.Embed = embed.Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(react.Emote, react.User.Value);
        }
    }
}
