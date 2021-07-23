using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Nix.MVC;
using System;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class NixCommandContext : SocketCommandContext
    {
        public NixClient NixClient { get; }
        public EmbedService Reply { get; }
        public ProcessServiceBase Process { get; }
        public IVoiceChannel VoiceChannel { get => (User as IVoiceState).VoiceChannel; }
        public ITextChannel TextChannel { get => Channel as ITextChannel; }

        private readonly INixProvider nixProvider;

        public NixCommandContext(
            DiscordSocketClient client,
            SocketUserMessage message, 
            IServiceProvider services) 
            : base(client, message)
        {
            NixClient = services.GetService<NixClient>();
            Reply = services.GetService<EmbedService>();
            Process = services.GetService<ProcessServiceBase>();
            nixProvider = services.GetService<INixProvider>();
        }

        public async Task<NixUser> GetNixUser()
        {
            return await nixProvider.Users.Get(User as SocketGuildUser);
        }

        public async Task<NixUser> GetNixUser(ulong id)
        {
            return await nixProvider.Users.Get(id, Guild.Id);
        }
    }
}
