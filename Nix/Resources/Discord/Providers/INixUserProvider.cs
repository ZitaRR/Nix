using Discord.WebSocket;
using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixUserProvider
    {
        Task<NixUser> Get(SocketGuildUser user);
        Task<NixUser> Get(ulong id, ulong guildId);
        Task<bool> Store(NixUser user);
        Task<bool> Remove(NixUser user);
        Task Update(NixUser user);
        Task<IEnumerable<NixUser>> GetAll();
    }
}
