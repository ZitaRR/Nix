using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixTrackProvider
    {
        Task<IEnumerable<NixTrack>> Get(NixUser user);
        Task<IEnumerable<NixTrack>> Get(ulong id);
        Task<bool> Store(NixTrack guild);
        Task<bool> Remove(NixTrack guild);
        Task Update(NixTrack guild);
        Task<IEnumerable<NixTrack>> GetAll();
    }
}
