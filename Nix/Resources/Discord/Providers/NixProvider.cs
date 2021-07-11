using System.Linq;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public class NixProvider : INixProvider
    {
        public INixGuildProvider Guilds { get; }
        public INixChannelProvider Channels { get; }
        public INixUserProvider Users { get; }

        public NixProvider(INixGuildProvider guilds, INixChannelProvider channels, INixUserProvider users)
        {
            Guilds = guilds;
            Channels = channels;
            Users = users;
        }

        public async Task<int> GetGuildsCountAsync()
        {
            return (await Guilds.GetAll()).Count();
        }

        public async Task<int> GetChannelsCountAsync()
        {
            return (await Channels.GetAll()).Count();
        }

        public async Task<int> GetUsersCountAsync()
        {
            return (await Users.GetAll()).Count();
        }
    }
}
