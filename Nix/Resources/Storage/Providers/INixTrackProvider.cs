using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixTrackProvider
    {
        Task<IEnumerable<NixTrack>> Get(NixUser user);
        Task<IEnumerable<NixTrack>> Get(ulong id);
        Task<bool> Store(NixTrack track);
        Task<bool> Remove(NixTrack track);
        Task Update(NixTrack track);
        Task<IEnumerable<NixTrack>> GetAll();
    }
}
