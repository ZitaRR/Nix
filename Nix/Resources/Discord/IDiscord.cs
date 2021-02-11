using Discord.WebSocket;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface IDiscord
    {
        DiscordSocketClient Client { get; }
        Task StartAsync();
        Task ProcessMessage(SocketMessage message);
        Task Dispose();
    }
}
