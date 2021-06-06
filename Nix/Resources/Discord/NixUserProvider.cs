using Nix.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class NixUserProvider : INixUserProvider
    {
        private readonly IPersistentStorage storage;

        public NixUserProvider(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task<NixUser> GetUser(ulong id, ulong guildId)
        {
            var user = await storage.FindOneAsync<NixUser>(
                new { DiscordId = id.ToString(), GuildId = guildId.ToString() });

            if (user is null)
                return null;

            return user;
        }

        public async Task Store(NixUser user)
        {
            if (await storage.ExistsAsync(user))
                return;

            await storage.InsertAsync(user);
        }

        public async Task Update(NixUser user)
        {
            if (!await storage.ExistsAsync(user))
            {
                await storage.InsertAsync(user);
                return;
            }

            await storage.UpdateAsync(user);
        }

        public async Task<IEnumerable<NixUser>> GetAllUsers()
        {
            return await storage.FindAllAsync<NixUser>();
        }
    }
}
