using Nix.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixTrackProvider : INixTrackProvider
    {
        private readonly IPersistentStorage storage;

        public NixTrackProvider(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task<IEnumerable<NixTrack>> Get(NixUser user)
        {
            return await Get(user.DiscordId.ToUlong());
        }

        public async Task<IEnumerable<NixTrack>> Get(ulong id)
        {
            return await storage.FindAsync<NixTrack>(
                new { DiscordId = id.ToString() });
        }

        public Task<bool> Store(NixTrack track)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(NixTrack track)
        {
            throw new NotImplementedException();
        }

        public Task Update(NixTrack track)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NixTrack>> GetAll()
        {
            return await storage.FindAllAsync<NixTrack>();
        }
    }
}
