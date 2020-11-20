using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
        private readonly IRegister register;

        public NixClient(ILogger logger, IPersistentStorage storage, IRegister register)
        {
            this.logger = logger;
            this.storage = storage;
            this.register = register;

            Client = new DiscordSocketClient();
            commands = new CommandService();

            services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(commands)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            Client.MessageReceived += ProcessMessage;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.Ready += OnReady;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await Client.LoginAsync(TokenType.Bot, Config.Data.Token);
            await Client.StartAsync();
        }

        private Task Client_LeftGuild(SocketGuild guild)
        {
            register.UnRegisterGuild(guild.GetNixGuild());
            return Task.CompletedTask;
        }

        private Task Client_JoinedGuild(SocketGuild guild)
        {
            register.RegisterGuild(guild.GetNixGuild());
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(SocketGuild guild)
        {
            register.RegisterGuild(guild.GetNixGuild());
            return Task.CompletedTask;
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
            logger.AppendLog($"{msg.Author.Username} >> {msg.Content}");

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Data.Prefix, ref argPos) || 
                msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);
                IResult result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                    logger.AppendLog(result.ErrorReason, LogSeverity.Debug);
            }
        }

        public async Task OnReady()
        {
#if DEBUG
            await Client.SetGameAsync("myself being created", type: ActivityType.Watching);
#endif
            var guilds = storage.FindAll<NixGuild>();
            var users = storage.FindAll<NixUser>();
            logger.AppendLog($"{guilds.Count()} guild(s) are registered with {users.Count()} user(s)");

            logger.AppendLog("Discord initialized");
        }

        private void HandleUser(SocketGuildUser user)
        {
            var nixUser = storage.FindOne<NixUser>(x => x.UserID == user.Id && x.GuildID == user.Guild.Id);
            if (nixUser is null)
                register.RegisterUser(nixUser = user.GetNixUser());

            nixUser.TotalMessages++;
            storage.Update(nixUser);
        }
    }
}
