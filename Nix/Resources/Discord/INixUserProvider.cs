using Nix.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public interface INixUserProvider
    {
        Task<NixUser> GetUser(ulong id, ulong guildId);
        Task Store(NixUser user);
        Task Update(NixUser user);
        Task<IEnumerable<NixUser>> GetAllUsers();
    }
}
