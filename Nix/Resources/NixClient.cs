using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix.Resources
{
    internal class NixClient : IDiscord
    {
        public DiscordSocketClient Client { get; private set; }

        private CommandService commands;
        private readonly IServiceProvider services;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;

        private const string SOURCE = "DISCORD";

        public NixClient(IServiceProvider services, ILogger logger, IPersistentStorage storage)
        {
            this.services = services;
            this.logger = logger;
            this.storage = storage;
        }

        public async Task StartAsync()
        {
            Client = new DiscordSocketClient();
            commands = new CommandService();

            Client.MessageReceived += ProcessMessage;
            Client.Ready += OnReady;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await Client.LoginAsync(TokenType.Bot, Config.Data.Token);
            await Client.StartAsync();
        }

        public async Task Dispose()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();
            Client = null;
        }

        public async Task ProcessMessage(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            if (msg is null || msg.Author.IsBot)
                return;

            HandleUser(msg.Author as SocketGuildUser);
            logger.AppendLog(LogSeverity.Info, $"{msg.Author.Username} said ‘{msg.Content}‚");

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Data.Prefix, ref argPos) || 
                msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);
                IResult result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                    logger.AppendLog(LogSeverity.Debug, result.ErrorReason);
            }
        }

        public async Task OnReady()
        {
#if DEBUG
            await Client.SetGameAsync("myself being created", type: ActivityType.Watching);
#endif
            logger.AppendLog(SOURCE, "Discord initialized");
        }

        private void HandleUser(SocketGuildUser user)
        {
            if (user is null)
                return;

            var nixUser = new NixUser
            {
                Name = user.Username, 
                ID = user.Id,
                CreatedAt = user.CreatedAt.DateTime,
                AvatarURL = user.GetAvatarUrl(),
                Roles = user.Roles
            };

            storage.Store(nixUser);
        }
    }
}
