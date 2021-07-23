﻿using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.MVC;
using Nix.Resources.Discord;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace Nix.Resources
{
    public class InputHandler
    {
        private readonly IDiscord discord;
        private readonly ILogger logger;
        private readonly INixProvider nixProvider;
        private readonly IServiceProvider services;
        private readonly CommandService commands = new CommandService();

        public InputHandler(
            IDiscord discord, 
            ILogger logger, 
            INixProvider nixProvider,
            LavalinkService lavalink,
            MinecraftService minecraft)
        {
            this.discord = discord;
            this.logger = logger;
            this.nixProvider = nixProvider;

            services = new ServiceCollection()
                .AddSingleton(discord)
                .AddSingleton(discord.Client)
                .AddSingleton(commands)
                .AddSingleton(logger)
                .AddSingleton(lavalink)
                .AddSingleton(minecraft)
                .AddSingleton<InteractiveService>()
                .AddSingleton<AudioService>()
                .AddSingleton<SpotifyService>()
                .AddSingleton<EmbedService>()
                .AddLavaNode(lava => lava.SelfDeaf = true)
                .BuildServiceProvider();
        }

        public async Task InitialiseAsync()
        {
            discord.Client.MessageReceived += ProcessMessageAsync;

            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
        }

        private async Task ProcessMessageAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message is null || message.Author.IsBot)
                return;

            logger.AppendLog($"{message.Author.Username} >> {message.Content}");

            int argPos = 0;
            IResult result = null;

            if (message.HasStringPrefix(Config.Data.Prefix, ref argPos) ||
                message.HasMentionPrefix(discord.Client.CurrentUser, ref argPos))
            {
                var context = new NixCommandContext(discord.Client, message, services);
                result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                {
                    await context.Reply.ErrorAsync(context.Channel as ITextChannel, result.ErrorReason);
                    logger.AppendLog(result.ErrorReason, LogSeverity.Warning);
                }
            }

            await ProcessUserAsync(message.Author as SocketGuildUser, result?.IsSuccess ?? false);
        }

        private async Task ProcessUserAsync(SocketGuildUser user, bool usedCommand = false)
        {
            NixUser nixUser = await nixProvider.Users.Get(user);

            if (usedCommand)
                nixUser.CommandsIssued++;

            nixUser.Messages++;
            await nixProvider.Users.Update(nixUser);
        }
    }
}
