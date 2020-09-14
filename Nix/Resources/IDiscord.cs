using Discord.WebSocket;
using System.Threading.Tasks;

namespace Nix.Resources
{
    interface IDiscord
    {
        DiscordSocketClient Client { get; }
        Task StartAsync();
        Task ProcessMessage(SocketMessage message);
        Task Dispose();
    }
}
