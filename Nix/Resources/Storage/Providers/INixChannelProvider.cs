using Discord.WebSocket;
using Nix.MVC; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixChannelProvider
    {
        Task<NixChannel> Get(SocketGuildChannel channel);
        Task<NixChannel> Get(ulong id, ulong guildId);
        Task<bool> Store(NixChannel channel);
        Task<bool> Remove(NixChannel channel);
        Task Update(NixChannel channel);
        Task<IEnumerable<NixChannel>> GetAll();
    }
}
