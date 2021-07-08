using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface INixProvider
    {
        public INixGuildProvider Guilds { get; }
        public INixChannelProvider Channels { get; }
        public INixUserProvider Users { get; }
        Task<int> GetGuildsCountAsync();
        Task<int> GetChannelsCountAsync();
        Task<int> GetUsersCountAsync();
    }
}
