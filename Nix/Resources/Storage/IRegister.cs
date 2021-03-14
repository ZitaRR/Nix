using Discord.WebSocket;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface IRegister
    {
        Task RegisterGuild(SocketGuild guild);
        Task UnregisterGuild(SocketGuild guild);
        Task RegisterUser(SocketGuildUser user);
        Task UnregisterUser(SocketGuildUser user);
        Task RegisterChannel(SocketGuildChannel channel);
        Task UnregisterChannel(SocketGuildChannel channel);
    }
}
