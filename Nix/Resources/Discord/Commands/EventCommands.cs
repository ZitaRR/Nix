using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nix.Resources.Discord.Commands
{
    public class EventCommands : InteractiveBase<NixCommandContext>
    {
        private readonly EventService eventService;
        private readonly EmbedService embed;
        private readonly TimeSpan timeout = new TimeSpan(0, 1, 30);

        public EventCommands(EventService eventService, EmbedService embed)
        {
            this.eventService = eventService;
            this.embed = embed;
        }

        [Command("event", RunMode = RunMode.Async)]
        public async Task EventAsync([Remainder] string name)
        {
            await Context.Message.DeleteAsync();
            var prompt = await ReplyAsync("***Enter the description of the event***\n" +
                "``none`` for an empty description");

            var response = await NextMessageAsync(timeout: timeout);
            if (response is null)
            {
                await embed.ErrorAsync(Context.Channel as ITextChannel, "You did not respond... Aborting event");
                return;
            }

            string description = response.Content.ToLower() == "none" ? "" : response.Content;

            await prompt.DeleteAsync();
            await response.DeleteAsync();

            prompt = await ReplyAsync("***Enter date and time for the event***\n" +
                "**Format** ``month/day hours:minutes``");

            response = await NextMessageAsync(timeout: timeout);
            if (response is null)
            {
                await embed.ErrorAsync(Context.Channel as ITextChannel, "You did not respond... Aborting event");
                return;
            }

            DateTime.TryParse("2020/" + response.Content, out var start);
            await prompt.DeleteAsync();
            await response.DeleteAsync();
            await eventService.CreateEvent(Context.Channel as ITextChannel, Context.User, name, description, start);
        }

        [Command("del")]
        public async Task DeleteAsync(int id)
        {
            await eventService.DeleteEvent(Context.Channel as ITextChannel, id);
        }
    }
}
