using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class EnsurePlayerConnection : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, 
            CommandInfo command, 
            IServiceProvider services)
        {
            var audio = services.GetService(typeof(AudioService)) as AudioService;
            if (!audio.TryGetPlayer(context.Guild, out _))
            {
                return Task.FromResult(PreconditionResult.FromError("I'm not connected to a voice-channel :x:"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
