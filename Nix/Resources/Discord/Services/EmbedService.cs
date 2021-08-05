using Discord;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nix.MVC;

namespace Nix.Resources.Discord
{
    public sealed class EmbedService
    {
        private readonly Color NormalColor;
        private readonly Color errorColor;
        private readonly InteractiveService interactive;
        private EmbedBuilder embed;

        public EmbedService(InteractiveService interactive)
        {
            this.interactive = interactive;
            NormalColor = new Color(254, 254, 254);
            errorColor = new Color(254, 50, 50);
        }

        public async Task ErrorAsync(ITextChannel channel, string message)
        {
            embed = new()
            {
                Description = message,
                Color = errorColor,
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task ExceptionAsync(ITextChannel channel, Exception e)
        {
            embed = new()
            {
                Description = $"**ERROR** ``{e.HResult}``\n{e.StackTrace}",
                Color = errorColor,
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task MessageAsync(ITextChannel channel, string message)
        {
            embed = new()
            {
                Description = message,
                Color = NormalColor,
            };

            await channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task PaginatedMessageAsync(NixCommandContext context, List<string> pages, string title = null)
        {
            PaginatedMessage pager = new()
            {
                Title = title,
                Pages = pages,
                Color = NormalColor,
                Options = new()
                {
                    JumpDisplayOptions = JumpDisplayOptions.Never,
                    DisplayInformationIcon = false
                }
            };

            await interactive.SendPaginatedMessageAsync(context, pager);
        }

        public async Task EventAsync(ITextChannel channel, NixEvent nixEvent)
        {
            throw new NotImplementedException($"{nameof(EventAsync)} has not implemented yet.");
        }

        public async Task EventUpdateAsync(ITextChannel channel, SocketReaction react, NixEvent nixEvent)
        {
            throw new NotImplementedException($"{nameof(EventUpdateAsync)} has not implemeted yet.");
        }
    }
}
