using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    [RequireVoice]
    public class AudioModule : ModuleBase<NixCommandContext>
    {
        public AudioService Audio { get; set; }

        [Command("join")]
        public async Task JoinAsync()
            => await Audio.JoinAsync(Context.User as IVoiceState, Context.Channel as ITextChannel);

        [AudioBindChannel]
        public class BindedAudioModule : AudioModule
        {
            [Command("leave")]
            public async Task LeaveAsync()
            => await Audio.LeaveAsync(Context.User as IVoiceState, Context.Channel as ITextChannel);

            [Command("play", RunMode = RunMode.Async)]
            public async Task PlayAsync([Remainder] string search)
                => await Audio.PlayAsync(Context.User as IVoiceState, Context.Channel as ITextChannel, search);

            [Command("duration")]
            public async Task DurationAsync()
                => await Audio.DurationAsync(Context.Channel as ITextChannel);

            [Command("skip")]
            public async Task SkipAsync(int amount = 1)
                => await Audio.SkipAsync(Context.Channel as ITextChannel, amount);

            [Command("current")]
            public async Task CurrentAsync()
                => await Audio.CurrentAsync(Context.Channel as ITextChannel);

            [Command("artwork")]
            public async Task ArtworkAsync()
                => await Audio.ArtworkAsync(Context.Channel as ITextChannel);

            [Command("queue")]
            public async Task ListQueueAsync()
                => await Audio.ListQueueAsync(Context);

            [Command("repeat")]
            public async Task RepeatAsync()
                => await Audio.RepeatAsync(Context.Channel as ITextChannel);

            [Command("volume")]
            public async Task VolumeAsync(ushort volume)
                => await Audio.VolumeAsync(Context.Channel as ITextChannel, volume);

            [Command("shuffle")]
            public async Task ShuffleAsync()
                => await Audio.ShuffleAsync(Context.Channel as ITextChannel);

            [Command("time")]
            public async Task SeekAsync(ushort seconds)
                => await Audio.SeekAsync(Context.Channel as ITextChannel, seconds);

            [Command("pause")]
            public async Task PauseAsync()
                => await Audio.PauseAsync(Context.Channel as ITextChannel);

            [Command("resume")]
            public async Task ResumeAsync()
                => await Audio.ResumeAsync(Context.Channel as ITextChannel);

            [Command("lyrics")]
            public async Task LyricsAsync()
                => await Audio.LyricsAsync(Context.Channel as ITextChannel);
        }
    }
}
