using Discord.Commands;
using Discord.WebSocket;

namespace Nix.Resources
{
    public class NixCommandContext : SocketCommandContext
    {
        public NixUser GetNixUser
            => storage.FindOne<NixUser>(x => x.UserID == User.Id && x.GuildID == Guild.Id);
        public NixChannel GetNixChannel
            => storage.FindOne<NixChannel>(x => x.ChannelID == Channel.Id);

        private readonly IPersistentStorage storage;
        private readonly ILogger logger;

        public NixCommandContext(DiscordSocketClient client, SocketUserMessage message,
            IPersistentStorage storage, ILogger logger) : base(client, message)
        {
            this.storage = storage;
            this.logger = logger;
            this.logger.AppendLog("NixCommandContext initialized");
        }
    }
}
