using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public class AudioModule : ModuleBase<NixCommandContext>
    {
        private readonly AudioService audio;

        public AudioModule(AudioService audio)
        {
            this.audio = audio;
        }

        [Command("join")]
        public async Task JoinAsync()
            => await audio.JoinAsync(Context.User as IVoiceState, Context.Channel as ITextChannel);

        [Command("leave")]
        public async Task LeaveAsync()
            => await audio.LeaveAsync(Context.User as IVoiceState, Context.Channel as ITextChannel);

        [Command("play")]
        public async Task PlayAsync([Remainder] string search)
            => await audio.PlayAsync(Context.User as IVoiceState, Context.Channel as ITextChannel, search);

        [Command("ytplay")]
        public async Task PlayYoutubeAsync([Remainder] string search)
            => await audio.PlayYoutubeAsync(Context.User as IVoiceState, Context.Channel as ITextChannel, search);

        [Command("duration")]
        public async Task DurationAsync()
            => await audio.DurationAsync(Context.Channel as ITextChannel);

        [Command("skip")]
        public async Task SkipAsync(int amount = 1)
            => await audio.SkipAsync(Context.User as IVoiceState, Context.Channel as ITextChannel, amount);

        [Command("current")]
        public async Task CurrentAsync()
            => await audio.CurrentAsync();

        [Command("artwork")]
        public async Task ArtworkAsync()
            => await audio.ArtworkAsync();

        [Command("queue")]
        public async Task ListQueueAsync()
            => await audio.ListQueueAsync(Context);

        [Command("repeat")]
        public async Task RepeatAsync()
            => await audio.RepeatAsync();

        [Command("volume")]
        public async Task VolumeAsync(ushort volume)
            => await audio.VolumeAsync(Context.User as IVoiceState, volume);
    }
}
