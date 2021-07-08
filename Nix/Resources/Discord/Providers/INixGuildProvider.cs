using Discord.WebSocket;
using Nix.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixGuildProvider
    {
        Task<NixGuild> Get(SocketGuild guild);
        Task<NixGuild> Get(ulong id);
        Task<bool> Store(NixGuild guild);
        Task<bool> Remove(NixGuild guild);
        Task Update(NixGuild guild);
        Task<IEnumerable<NixGuild>> GetAll();
    }
}
