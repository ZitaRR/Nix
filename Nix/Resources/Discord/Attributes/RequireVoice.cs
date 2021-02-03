using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class RequireVoice : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                if (user.VoiceState is null)
                {
                    return Task.FromResult(PreconditionResult.FromError("You must be connected to a voice-channel"));
                }

                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("You must be in a server"));
            }
        }
    }
}
