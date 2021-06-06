using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.Controllers;
using Nix.Resources;
using Nix.Models;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Nix.Resources.Discord;

namespace Nix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = $"{Name()} v{Version()}";
            Extensions.AddServices(new ServiceCollection()).BuildServiceProvider().GetService<HomeController>();
        }

        public static string Version()
            => FileVersionInfo.GetVersionInfo(
                Assembly.GetExecutingAssembly().Location).ProductVersion;

        public static string Name()
            => Assembly.GetExecutingAssembly().GetName().Name;

        public static string ConnectionString()
        {
            string name;
#if DEBUG
            name = "DevNixDB";
#else
            name = "NixDB";
#endif
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }

    static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection collection)
            => collection.AddSingleton<IDiscord, NixClient>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IPersistentStorage, PersistentStorage>()
                .AddSingleton<IRegister, Register>()
                .AddSingleton<INixUserProvider, NixUserProvider>()
                .AddSingleton<HomeController>()
                .AddSingleton<SettingsController>()
                .AddSingleton<DiscordController>();

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
                DiscordId = user.Id.ToString(),
                GuildId = user.Guild.Id.ToString(),
                AvatarUrl = user.GetAvatarUrl(),
                CreatedAt = user.CreatedAt.DateTime,
                JoinedAt = user.JoinedAt.GetValueOrDefault().DateTime,
                TotalMessages = 0
            };
        }

        public static NixGuild GetNixGuild(this SocketGuild guild)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));

            return new NixGuild
            {
                Name = guild.Name,
                DiscordId = guild.Id.ToString()
            };
        }

        public static NixChannel GetNixChannel(this SocketGuildChannel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            return new NixChannel
            {
                Name = channel.Name,
                DiscordId = channel.Id.ToString(),
                GuildId = channel.Guild.Id.ToString()
            };
        }
    }
}
