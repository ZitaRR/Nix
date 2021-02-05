using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.Resources.Discord;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace Nix.Resources
{
    public class NixClient : IDiscord
    {
        public DiscordSocketClient Client { get; private set; }
        public Stopwatch Watch { get; private set; } = new Stopwatch();
        public OperatingSystem OS { get; } = Environment.OSVersion;

        private CommandService commands;
        private readonly IServiceProvider services;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;
        private readonly IRegister register;
        private EventService eventService;

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
                .AddSingleton(storage)
                .AddSingleton(logger)
                .AddSingleton<InteractiveService>()
                .AddSingleton<AudioService>()
                .AddSingleton<EmbedService>()
                .AddSingleton<EventService>()
                .AddSingleton<ScriptService>()
                .AddLavaNode(x => x.SelfDeaf = true)
                .BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            Client.MessageReceived += ProcessMessage;
            Client.ReactionAdded += Client_ReactionAdded;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.Ready += OnReady;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await Client.LoginAsync(TokenType.Bot, Config.Data.Token);
            await Client.StartAsync();
            Watch.Start();
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cache,
            ISocketMessageChannel channel, SocketReaction reaction)
        {
            try
            {
                await eventService.UpdateEvent(channel as ITextChannel, reaction);
            }
            catch(Exception e)
            {
                logger.AppendLog(e);
            }
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
                var context = new NixCommandContext(Client, msg, storage,
                    this, services.GetRequiredService<EmbedService>(),
                    services.GetRequiredService<ScriptService>());
                IResult result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                {
                    await context.Reply.ErrorAsync(context.Channel as ITextChannel, result.ErrorReason);
                    logger.AppendLog(result.ErrorReason, LogSeverity.Warning);
                }
            }
        }

        public async Task OnReady()
        {
            var guilds = storage.FindAll<NixGuild>();
            var users = storage.FindAll<NixUser>();
            logger.AppendLog($"{guilds.Count()} guild(s) are registered with {users.Count()} user(s)");

#if DEBUG
            await Client.SetGameAsync("myself being created", type: ActivityType.Watching);
#else
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var totalUsers = Client.Guilds.Sum(x => x.Users.Count);
                    await Client.SetGameAsync($"{totalUsers} users | v{Program.Version()}", type: ActivityType.Listening);
                    await Task.Delay((1000 * 60) * 15);
                }
            });
#endif

            eventService = services.GetRequiredService<EventService>();
            var lavaNode = services.GetRequiredService<LavaNode>();
            if (!lavaNode.IsConnected)
                await lavaNode.ConnectAsync();

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
