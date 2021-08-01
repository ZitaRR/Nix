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
        public EmbedService Reply { get; }
        public IVoiceChannel VoiceChannel { get => (User as IVoiceState).VoiceChannel; }
        public ITextChannel TextChannel { get => Channel as ITextChannel; }

        private readonly INixProvider nixProvider;

        public NixCommandContext(
            DiscordSocketClient client,
            SocketUserMessage message, 
            IServiceProvider services) 
            : base(client, message)
        {
            Reply = services.GetService<EmbedService>();
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
