using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace Nix.Resources.Discord
{
    public class Audio : ModuleBase<NixCommandContext>
    {
        private readonly AudioService audio;

        public Audio(AudioService audio)
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

        [Command("duration")]
        public async Task DurationAsync()
            => await audio.DurationAsync(Context.Channel as ITextChannel);

        [Command("skip")]
        public async Task SkipAsync()
            => await audio.SkipAsync(Context.User as IVoiceState, Context.Channel as ITextChannel);

        [Command("current")]
        public async Task CurrentAsync()
            => await audio.CurrentAsync();

        [Command("artwork")]
        public async Task ArtworkAsync()
            => await audio.ArtworkAsync();
    }
}
