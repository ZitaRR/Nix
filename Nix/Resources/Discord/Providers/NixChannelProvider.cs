using Discord.WebSocket;
using Nix.MVC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixChannelProvider : INixChannelProvider
    {
        private readonly IPersistentStorage storage;

        public NixChannelProvider(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task<NixChannel> Get(SocketGuildChannel channel)
        {
            NixChannel nixChannel = await Get(channel.Id, channel.Guild.Id);

            if (nixChannel is null)
            {
                nixChannel = channel.GetNixChannel();
                await Store(nixChannel);
                return nixChannel;
            }

            return nixChannel;
        }

        public async Task<NixChannel> Get(ulong id, ulong guildId)
        {
            return await storage.FindOneAsync<NixChannel>(
                new { Id = id.ToString(), GuildId = guildId.ToString() });
        }

        public async Task<bool> Store(NixChannel channel)
        {
            if (await storage.ExistsAsync<NixChannel>(CreateProperties(channel)))
            {
                return false;
            }

            await storage.InsertAsync(channel);
            return true;
        }

        public async Task<bool> Remove(NixChannel channel)
        {
            if (!await storage.ExistsAsync<NixChannel>(CreateProperties(channel)))
            {
                return false;
            }

            await storage.DeleteAsync<NixChannel>(CreateProperties(channel));
            return true;
        }

        public async Task Update(NixChannel channel)
        {
            if (await Store(channel))
            {
                return;
            }

            await storage.UpdateAsync(channel);
        }

        public async Task<IEnumerable<NixChannel>> GetAll()
        {
            return await storage.FindAllAsync<NixChannel>();
        }

        private object CreateProperties(NixChannel channel)
        {
            return new { Id = channel.Id.ToString(), GuildId = channel.GuildId.ToString() };
        }
    }
}
