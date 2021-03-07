using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class AudioBindChannel : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var audio = services.GetService(typeof(AudioService)) as AudioService;
            var nix = audio.GetPlayer(context.Guild.Id);

            if (nix != null &&
                nix?.TextChannel?.Id != context.Channel.Id)
                return Task.FromResult(PreconditionResult
                        .FromError($"I'm bound to {nix.TextChannel.Name}"));
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
