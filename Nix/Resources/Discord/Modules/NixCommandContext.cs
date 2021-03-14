using Discord.Commands;
using Discord.WebSocket;
using Nix.Models;

namespace Nix.Resources.Discord
{
    public class NixCommandContext : SocketCommandContext
    {
        public IPersistentStorage Storage { get; }
        public NixClient NixClient { get; }
        public EmbedService Reply { get; }
        public ScriptService Script { get; }

        public NixCommandContext(DiscordSocketClient client,
            SocketUserMessage message,
            IPersistentStorage storage, 
            NixClient nixClient,
            EmbedService reply, 
            ScriptService script) 
            : base(client, message)
        {
            Storage = storage;
            NixClient = nixClient;
            Reply = reply;
            Script = script;
        }
    }
}
