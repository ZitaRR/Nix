using Discord.WebSocket;
using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixGuildProvider : INixGuildProvider
    {
        private readonly IPersistentStorage storage;

        public NixGuildProvider(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task<NixGuild> Get(SocketGuild guild)
        {
            NixGuild nixGuild = await Get(guild.Id);

            if (nixGuild is null)
            {
                nixGuild = guild.GetNixGuild();
                await Store(nixGuild);
                return nixGuild;
            }

            return nixGuild;
        }

        public async Task<NixGuild> Get(ulong id)
        {
            return await storage.FindOneAsync<NixGuild>(new { Id = id.ToString() });
        }

        public async Task<bool> Store(NixGuild guild)
        {
            if (await storage.ExistsAsync<NixGuild>(CreateProperties(guild)))
            {
                return false;
            }

            await storage.InsertAsync(guild);
            return true;
        }

        public async Task<bool> Remove(NixGuild guild)
        {
            if (!await storage.ExistsAsync<NixGuild>(CreateProperties(guild)))
            {
                return false;
            }

            await storage.DeleteAsync<NixGuild>(CreateProperties(guild));
            return true;
        }

        public async Task Update(NixGuild guild)
        {
            if (await Store(guild))
            {
                return;
            }

            await storage.UpdateAsync(guild);
        }

        public async Task<IEnumerable<NixGuild>> GetAll()
        {
            return await storage.FindAllAsync<NixGuild>();
        }

        private object CreateProperties(NixGuild guild)
        {
            return new { Id = guild.Id.ToString() };
        }
    }
}
