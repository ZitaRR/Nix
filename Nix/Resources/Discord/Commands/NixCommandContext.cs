using Discord.Commands;
using Discord.WebSocket;

namespace Nix.Resources.Discord
{
    public class NixCommandContext : SocketCommandContext
    {
        public NixUser GetNixUser
            => storage.FindOne<NixUser>(x => x.UserID == User.Id && x.GuildID == Guild.Id);
        public NixChannel GetNixChannel
            => storage.FindOne<NixChannel>(x => x.ChannelID == Channel.Id);
        public NixClient NixClient { get; }
        public ReplyService Reply { get; }

        private readonly IPersistentStorage storage;

        public NixCommandContext(DiscordSocketClient client, SocketUserMessage message,
            IPersistentStorage storage, NixClient nixClient,
            ReplyService reply) : base(client, message)
        {
            this.storage = storage;
            NixClient = nixClient;
            Reply = reply;
        }
    }
}
