using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            UnregisterGuildAndItsUsers(guild.GetNixGuild());
            return Task.CompletedTask;
        }

        private Task Client_JoinedGuild(SocketGuild guild)
        {
            RegisterGuildAndItsUsers(guild.GetNixGuild());
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(SocketGuild guild)
        {
            RegisterGuildAndItsUsers(guild.GetNixGuild());
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
            logger.AppendLog($"{msg.Author.Username} said ‘{msg.Content}‚");

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
            var users = guilds.Select(x => x.Users).Sum(x => x.Count);
            logger.AppendLog($"{guilds.Count()} guilds are registered with {users} users");

            logger.AppendLog("Discord initialized");
        }

        private void HandleUser(SocketGuildUser user)
        {
            var nixUser = storage.FindOne<NixUser>(x => x.UserID == user.Id && x.GuildID == user.Guild.Id);
            if (nixUser is null)
                storage.Store(nixUser = user.GetNixUser());

            nixUser.TotalMessages++;
            storage.Update(nixUser);
        }

        private void RegisterGuildAndItsUsers(NixGuild guild)
        {
            if (storage.Exists<NixGuild>(x => x.GuildID == guild.GuildID))
                return;

            storage.Store(guild);

            int registeredUsers = 0;
            foreach (var user in guild.Users)
            {
                if (storage.Exists<NixUser>(x => x.UserID == user.UserID && x.GuildID == user.GuildID))
                    continue;

                registeredUsers++;
                storage.Store(user);
            }

            logger.AppendLog($"{guild.Name} registered along with {registeredUsers} users");
        }

        private void UnregisterGuildAndItsUsers(NixGuild guild)
        {
            storage.Delete<NixGuild>(x => x.GuildID == guild.GuildID);

            int unregisteredUsers = 0;
            foreach (var user in guild.Users)
            {
                try
                {
                    storage.Delete<NixUser>(x => x.UserID == user.UserID && x.GuildID == guild.GuildID);
                }
                catch { continue; }
                unregisteredUsers++;
            }

            logger.AppendLog($"{guild.Name} unregistered along with {unregisteredUsers} users");
        }
    }
}
