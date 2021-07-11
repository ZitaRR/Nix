using Discord.WebSocket;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class Register : IRegister
    {
        private readonly INixProvider nixProvider;

        public Register(INixProvider nixProvider)
        {
            this.nixProvider = nixProvider;
        }

        public async Task RegisterGuild(SocketGuild guild)
        {
            await nixProvider.Guilds.Store(guild.GetNixGuild());

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
            if (channel is SocketCategoryChannel)
                return;

            await nixProvider.Channels.Store(channel.GetNixChannel());
        }

        public async Task RegisterUser(SocketGuildUser user)
        {
            await nixProvider.Users.Store(user.GetNixUser());
        }

        public async Task UnregisterGuild(SocketGuild guild)
        {
            await nixProvider.Guilds.Remove(guild.GetNixGuild());

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
            await nixProvider.Channels.Remove(channel.GetNixChannel());
        }

        public async Task UnregisterUser(SocketGuildUser user)
        {
            await nixProvider.Users.Remove(user.GetNixUser());
        }
    }
}
