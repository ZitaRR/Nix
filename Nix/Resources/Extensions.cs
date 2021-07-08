using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.MVC;
using System;

namespace Nix.Resources
{
    public static class Extensions
    {
        public static IServiceCollection AddNixServices(this IServiceCollection collection)
        {
            return collection.AddSingleton<HomeController>()
                .AddSingleton<DiscordController>()
                .AddSingleton<SettingsController>()
                .AddSingleton<IDiscord, NixDiscord>()
                .AddSingleton<NixClient>()
                .AddSingleton<InputHandler>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IPersistentStorage, PersistentStorage>()
                .AddSingleton<IRegister, Register>()
                .AddSingleton<INixUserProvider, NixUserProvider>()
                .AddSingleton<INixChannelProvider, NixChannelProvider>()
                .AddSingleton<INixGuildProvider, NixGuildProvider>()
                .AddSingleton<INixProvider, NixProvider>();
        }

        public static ulong ToUlong(this string s)
        {
            ulong.TryParse(s, out ulong result);
            return result;
        }

        public static NixUser GetNixUser(this SocketGuildUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return new NixUser
            {
                Name = user.Username,
                Id = user.Id.ToString(),
                GuildId = user.Guild.Id.ToString(),
                AvatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(),
                CreatedAt = user.CreatedAt.DateTime,
                JoinedAt = user.JoinedAt.GetValueOrDefault().DateTime,
                Messages = 0
            };
        }

        public static NixGuild GetNixGuild(this SocketGuild guild)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            return new NixGuild
            {
                Name = guild.Name,
                Id = guild.Id.ToString(),
                CreatedAt = guild.CreatedAt.DateTime
            };
        }

        public static NixChannel GetNixChannel(this SocketGuildChannel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));
            else if (channel is SocketCategoryChannel)
                return null;

            return new NixChannel
            {
                Name = channel.Name,
                Id = channel.Id.ToString(),
                GuildId = channel.Guild.Id.ToString(),
                CreatedAt = channel.CreatedAt.DateTime
            };
        }
    }
}
