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
        private IServiceProvider services;
        private ILogger logger;

        public NixClient(IServiceProvider services, ILogger logger)
        {
            this.services = services;
            this.logger = logger;
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

            logger.AppendLog(LogSeverity.Info, $"{msg.Author.Username} said ‘{msg.Content}‚");

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Data.Prefix, ref argPos) || 
                msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);
                IResult result = await commands.ExecuteAsync(context, argPos, services);
            }
        }

        public async Task OnReady()
        {
#if DEBUG
            await Client.SetGameAsync("myself being created", type: ActivityType.Watching);
#endif
        }
    }
}
