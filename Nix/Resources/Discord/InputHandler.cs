using Discord;
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
        private readonly LavaNode lava;
        private readonly AudioService audio;

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

            lava = services.GetService<LavaNode>();
            audio = services.GetService<AudioService>();
        }

        public async Task InitialiseAsync()
        {
            discord.Client.Ready += OnReady;
            discord.Client.MessageReceived += ProcessMessageAsync;
            discord.Client.UserVoiceStateUpdated += OnUserVoiceStateUpdate;

            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
        }

        private async Task OnReady()
        {
            if (!lava.IsConnected)
                await lava.ConnectAsync();
        }

        private async Task OnUserVoiceStateUpdate(SocketUser user, SocketVoiceState origin, SocketVoiceState destination)
        {
            if (user.IsBot)
                return;

            var nix = origin.VoiceChannel?.GetUser(discord.Client.CurrentUser.Id) ??
                destination.VoiceChannel?.GetUser(discord.Client.CurrentUser.Id);

            if (nix is null)
                return;

            if (nix.VoiceChannel.Id == destination.VoiceChannel?.Id &&
                nix.VoiceChannel.Users.Count == 2)
                await audio.CancelDisconnect(nix.VoiceChannel.Guild);
            else if (nix.VoiceChannel.Users.Count == 1)
                _ = Task.Run(() => audio.InitiateDisconnectAsync(nix.VoiceChannel.Guild));
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
