using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixDiscord : IDiscord
    {
        public DiscordSocketClient Client { get; private set; } = new DiscordSocketClient();

        public NixDiscord()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                _ = Dispose();
            };
        }

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Data.Token))
                throw new NullReferenceException(nameof(Config.Data.Token));

            await Client.LoginAsync(TokenType.Bot, Config.Data.Token);
            await Client.StartAsync();
        }

        public async Task Dispose()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();
        }
    }
}
