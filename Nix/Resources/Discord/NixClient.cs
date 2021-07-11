using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixClient 
    {
        public Stopwatch Watch { get; private set; } = new Stopwatch();

        private readonly IDiscord discord;
        private readonly ILogger logger;
        private readonly INixProvider nixProvider;
        private readonly IRegister register;
        private readonly InputHandler inputHandler;

        public NixClient(
            IDiscord discord,
            ILogger logger,
            INixProvider nixProvider,
            IRegister register,
            InputHandler inputHandler)
        {
            this.discord = discord;
            this.logger = logger;
            this.nixProvider = nixProvider;
            this.register = register;
            this.inputHandler = inputHandler;
        }

        public async Task StartAsync()
        {
            discord.Client.GuildAvailable += Client_GuildAvailable;
            discord.Client.JoinedGuild += Client_JoinedGuild;
            discord.Client.LeftGuild += Client_LeftGuild;
            discord.Client.Disconnected += OnDisconnection;
            discord.Client.Ready += OnReady;

            await inputHandler.InitialiseAsync();
            await discord.StartAsync();
        }

        private async Task Client_LeftGuild(SocketGuild guild)
        {
            await register.UnregisterGuild(guild);
        }

        private async Task Client_JoinedGuild(SocketGuild guild)
        {
            await register.RegisterGuild(guild);
        }

        private async Task Client_GuildAvailable(SocketGuild guild)
        {
            await register.RegisterGuild(guild);
        }

        private Task OnDisconnection(Exception e)
        {
            Watch.Stop();
            return Task.CompletedTask;
        }

        private async Task OnReady()
        {
            int guilds = await nixProvider.GetGuildsCountAsync();
            int users = await nixProvider.GetUsersCountAsync();
            logger.AppendLog($"{guilds} guild(s) are registered with {users} user(s)");

#if DEBUG
            await discord.Client.SetGameAsync("myself being created", type: ActivityType.Watching);
#else
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await discord.Client.SetGameAsync($"{users} users | v{Utility.Version}", type: ActivityType.Listening);
                    await Task.Delay((1000 * 60) * 15);
                    users = await nixProvider.GetUsersCountAsync();
                }
            });
#endif

            logger.AppendLog("Discord initialized");
            Watch.Start();
        }
    }
}
