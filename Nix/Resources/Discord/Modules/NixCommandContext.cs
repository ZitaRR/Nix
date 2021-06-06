using Discord.Commands;
using Discord.WebSocket;
using Nix.Models;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class NixCommandContext : SocketCommandContext
    {
        public NixClient NixClient { get; }
        public EmbedService Reply { get; }
        public ScriptService Script { get; }

        private readonly INixUserProvider userProvider;

        public NixCommandContext(DiscordSocketClient client,
            SocketUserMessage message, 
            NixClient nixClient,
            EmbedService reply, 
            ScriptService script,
            INixUserProvider userProvider) 
            : base(client, message)
        {
            NixClient = nixClient;
            Reply = reply;
            Script = script;
            this.userProvider = userProvider;
        }

        public async Task<NixUser> GetNixUser()
        {
            return await userProvider.GetUser(User.Id, Guild.Id);
        }

        public async Task<NixUser> GetNixUser(ulong id)
        {
            return await userProvider.GetUser(id, Guild.Id);
        }
    }
}
