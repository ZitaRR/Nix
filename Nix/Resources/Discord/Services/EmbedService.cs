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
        private readonly DiscordSocketClient client;
        private readonly InteractiveService interactive;
        private readonly IPersistentStorage storage;
        private EmbedBuilder embed;

        public EmbedFooterBuilder Footer
            => new EmbedFooterBuilder
            {
                Text = $"{DateTime.UtcNow:yyyy\\-MM\\-dd} - {DateTime.UtcNow:HH\\:mm} UTC ◈ Latency: {client.Latency}ms",
                IconUrl = client.CurrentUser.GetAvatarUrl()
            };

        public EmbedService(DiscordSocketClient client, InteractiveService interactive, IPersistentStorage storage)
        {
            this.client = client;
            this.interactive = interactive;
            this.storage = storage;
            NormalColor = new Color(254, 254, 254);
            errorColor = new Color(254, 50, 50);
            length = 25;
        }

        private string FormatTrackTitle(string title)
            => title.Length > length ? title.Substring(0, length) + "..." : title;

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

        public async Task EventAsync(ITextChannel channel, NixEvent nixEvent)
        {
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

            nixEvent.MessageID = message.Id;
            storage.Update(nixEvent);
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
