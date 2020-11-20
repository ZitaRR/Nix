namespace Nix.Resources
{
    public class Register : IRegister
    {
        private readonly IPersistentStorage storage;
        private readonly ILogger logger;

        public Register(IPersistentStorage storage, ILogger logger)
        {
            this.storage = storage;
            this.logger = logger;
        }

        public void RegisterChannel(NixChannel channel)
        {
            if (storage.Exists<NixChannel>(x => x.ChannelID == channel.ChannelID))
                return;

            storage.Store(channel);
        }

        public void RegisterGuild(NixGuild guild)
        {
            foreach (var channel in guild.Channels)
            {
                RegisterChannel(channel);
            }

            if (storage.Exists<NixGuild>(x => x.GuildID == guild.GuildID))
                return;

            RegisterUser(guild.Client);
            storage.Store(guild);
        }

        public void RegisterUser(NixUser user)
        {
            if (storage.Exists<NixUser>(x => x.UserID == user.UserID && x.GuildID == user.GuildID))
                return;

            storage.Store(user);
        }

        public void UnregisterChannel(NixChannel channel)
            => storage.Delete<NixChannel>(x => x.ChannelID == channel.ChannelID);

        public void UnRegisterGuild(NixGuild guild)
        {
            UnregisterUser(guild.Client);
            int unregisteredUsers = 1;
            int unregisteredChannels = 0;

            foreach (var user in guild.Users)
            {
                UnregisterUser(user);
                unregisteredUsers++;
            }

            foreach (var channel in guild.Channels)
            {
                UnregisterChannel(channel);
                unregisteredChannels++;
            }

            storage.Delete<NixGuild>(x => x.GuildID == guild.GuildID);
            logger.AppendLog($"{guild.Name} unregistered along with " +
                $"{unregisteredChannels} channel(s) and " +
                $"{unregisteredUsers} user(s)");
        }

        public void UnregisterUser(NixUser user)
            => storage.Delete<NixUser>(x => x.UserID == user.UserID && x.GuildID == user.GuildID);
    }
}
