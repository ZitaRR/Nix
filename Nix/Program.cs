using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.Controllers;
using Nix.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;
            Extensions.AddServices(new ServiceCollection()).BuildServiceProvider().GetService<HomeController>();
        }
    }

    static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection collection)
            => collection.AddSingleton<IDiscord, NixClient>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IPersistentStorage, PersistentStorage>()
                .AddSingleton(x => new HomeController(x))
                .AddSingleton<SettingsController>()
                .AddSingleton<DiscordController>();

        public static IEnumerable<NixRole> GetNixRoles(this IReadOnlyCollection<SocketRole> roles)
        {
            if (roles is null)
                throw new ArgumentNullException(nameof(roles));
            else if (roles.Count <= 0)
                throw new ArgumentOutOfRangeException(nameof(roles));

            var list = new List<NixRole>();
            foreach (var role in roles)
            {
                list.Add(new NixRole 
                { 
                    RoleID = role.Id,
                    Name = role.Name 
                });
            }
            return list;
        }

        public static NixUser GetNixUser(this SocketGuildUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return new NixUser
            {
                Name = user.Username,
                UserID = user.Id,
                GuildID = user.Guild.Id,
                AvatarURL = user.GetAvatarUrl(),
                CreatedAt = user.CreatedAt.DateTime,
                JoinedAt = user.JoinedAt.GetValueOrDefault().DateTime,
                Roles = user.Roles.GetNixRoles(),
                TotalMessages = 1
            };
        }

        public static NixGuild GetNixGuild(this SocketGuild guild)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            var users = new List<NixUser>();
            foreach (var user in guild.Users)
            {
                users.Add(user.GetNixUser());
            }

            var channels = new List<NixChannel>();
            foreach (var channel in guild.Channels)
            {
                if (channel is ISocketMessageChannel)
                    channels.Add(channel.GetNixChannel());
            }

            return new NixGuild
            {
                Name = guild.Name,
                GuildID = guild.Id,
                Users = users,
                Channels = channels
            };
        }

        public static NixChannel GetNixChannel(this SocketGuildChannel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            return new NixChannel
            {
                Name = channel.Name,
                ChannelID = channel.Id,
                GuildID = channel.Guild.Id
            };
        }
    }
}
