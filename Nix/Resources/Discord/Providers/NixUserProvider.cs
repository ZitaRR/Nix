using Discord.WebSocket;
using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixUserProvider : INixUserProvider
    {
        private readonly IPersistentStorage storage;

        public NixUserProvider(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task<NixUser> Get(SocketGuildUser user)
        {
            NixUser nixUser = await Get(user.Id, user.Guild.Id);

            if (nixUser is null)
            {
                nixUser = user.GetNixUser();
                await Store(nixUser);
                return nixUser;
            }

            return nixUser;
        }

        public async Task<NixUser> Get(ulong id, ulong guildId)
        {
            return await storage.FindOneAsync<NixUser>(
                new { Id = id.ToString(), GuildId = guildId.ToString() });
        }

        public async Task<bool> Store(NixUser user)
        {
            if (await storage.ExistsAsync<NixUser>(CreateProperties(user)))
            {
                return false;
            }

            await storage.InsertAsync(user);
            return true;
        }

        public async Task<bool> Remove(NixUser user)
        {
            if (!await storage.ExistsAsync<NixUser>(CreateProperties(user)))
            {
                return false;
            }

            await storage.DeleteAsync<NixUser>(CreateProperties(user));
            return true;
        }

        public async Task Update(NixUser user)
        {
            if (await Store(user))
            {
                return;
            }

            await storage.UpdateAsync(user);
        }

        public async Task<IEnumerable<NixUser>> GetAll()
        {
            return await storage.FindAllAsync<NixUser>();
        }

        private object CreateProperties(NixUser user)
        {
            return new { Id = user.Id.ToString(), GuildId = user.GuildId.ToString() };
        }
    }
}
