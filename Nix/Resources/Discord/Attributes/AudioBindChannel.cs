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
            if (audio.TryGetPlayer(context.Guild, out NixPlayer nix))
            {
                if (nix.TextChannel.Id == context.Channel.Id)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }

                return Task.FromResult(PreconditionResult
                    .FromError($"I'm bound to {nix.TextChannel.Name}"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
