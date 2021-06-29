using Discord.WebSocket;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class Register : IRegister
    {
        private readonly IPersistentStorage storage;

        public Register(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        public async Task RegisterGuild(SocketGuild guild)
        {
            var nixGuild = guild.GetNixGuild();

            if (await storage.ExistsAsync(nixGuild))
                return;

            await storage.InsertAsync(nixGuild);

            foreach (var channel in guild.Channels)
            {
                await RegisterChannel(channel);
            }

            foreach (var user in guild.Users)
            {
                await RegisterUser(user);
            }
        }

        public async Task RegisterChannel(SocketGuildChannel channel)
        {
            var nixChannel = channel.GetNixChannel();

            if (nixChannel is null)
                return;
            else if (await storage.ExistsAsync(nixChannel))
                return;

            await storage.InsertAsync(nixChannel);
        }

        public async Task RegisterUser(SocketGuildUser user)
        {
            var nixUser = user.GetNixUser();

            if (await storage.ExistsAsync(nixUser))
                return;

            await storage.InsertAsync(nixUser);
        }

        public async Task UnregisterGuild(SocketGuild guild)
        {
            var nixGuild = guild.GetNixGuild();

            if (!await storage.ExistsAsync(nixGuild))
                return;

            await storage.DeleteAsync(nixGuild);

            foreach (var channel in guild.Channels)
            {
                await UnregisterChannel(channel);
            }

            foreach (var user in guild.Users)
            {
                await UnregisterUser(user);
            }
        }

        public async Task UnregisterChannel(SocketGuildChannel channel)
        {
            var nixChannel = channel.GetNixChannel();

            if (!await storage.ExistsAsync(nixChannel))
                return;

            await storage.DeleteAsync(nixChannel);
        }

        public async Task UnregisterUser(SocketGuildUser user)
        {
            var nixUser = user.GetNixUser();

            if (!await storage.ExistsAsync(nixUser))
                return;

            await storage.DeleteAsync(nixUser);
        }
    }
}
