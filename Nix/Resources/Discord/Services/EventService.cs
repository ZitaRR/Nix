using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Nix.Resources.Discord
{
    public sealed class EventService
    {
        private const int INTERVAL = 10000;

        private readonly IPersistentStorage storage;
        private readonly ILogger logger;
        private readonly EmbedService embed;
        private readonly DiscordSocketClient client;
        private Timer timer = new Timer();

        public EventService(IPersistentStorage storage, ILogger logger,
            EmbedService embed, DiscordSocketClient client)
        {
            this.storage = storage;
            this.logger = logger;
            this.embed = embed;
            this.client = client;

            timer.Elapsed += OnElapsed;
            timer.AutoReset =true;
            timer.Interval = INTERVAL;
            timer.Start();
        }

        public async Task CreateEvent(ITextChannel channel, SocketUser creator, string name, string description, DateTime start)
        {
            int eventId = storage.FindAll<NixEvent>().Count() + 1;
            ulong id = await embed.EventAsync(channel, creator, name, description, eventId, start);
            var user = storage.FindOne<NixUser>(x => creator.Id == x.UserID && x.GuildID == channel.GuildId);
            storage.Store(new NixEvent
            {
                Name = name,
                Creator = user,
                GuildID = channel.GuildId,
                Start = start,
                MessageID = id,
                Description = description
            });
        }

        public async Task UpdateEvent(ITextChannel channel, ulong messageId, ulong userId)
        {
            if (userId == client.CurrentUser.Id)
                return;
            
            var nixEvent = storage.FindOne<NixEvent>(x => messageId == x.MessageID && channel.GuildId == x.GuildID);
            var nixUser = storage.FindOne<NixUser>(x => userId == x.UserID && channel.GuildId == x.GuildID);
            
            if (nixEvent is null)
                return;
            if (nixEvent.Participants.Where(x => x.UserID == userId).Count() > 0)
                return;
            
            nixEvent.Participants.Add(nixUser);
            storage.Update(nixEvent);
            
            await embed.EventUpdateAsync(channel, nixEvent);
        }

        public async Task DeleteEvent(ITextChannel channel, int id)
        {
            var nixEvent = storage.FindOne<NixEvent>(x => x.ID == id);
            if (nixEvent is null)
            {
                await embed.ErrorAsync(channel, $"No event with the id ``{id}`` exists.");
                return;
            }

            storage.Delete<NixEvent>(x => x.ID == id);
            await embed.MessageAsync(channel, $"Deleted event ``{nixEvent.Name} [{nixEvent.ID}]``");
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            logger.AppendLog("Scanning for events...");
            var events = storage.FindAll<NixEvent>().ToList();
            if (events.Count() <= 0)
                return;
            logger.AppendLog($"Found {events.Count()} event(s)");

            for (int i = 0; i < events.Count(); i++)
            {
                var diff = events[i].Start - DateTime.UtcNow;
                logger.AppendLog(diff.ToString());
                if (diff > new TimeSpan(0, 10, 0))
                    continue;
                else if (diff < new TimeSpan(0))
                {
                    ulong guildId = events[i].Creator.GuildID;
                    storage.Delete<NixEvent>(x => x.GuildID == guildId);
                    continue;
                }

                foreach (var user in events[i].Participants)
                {
                    var discordUser = client.GetUser(user.UserID);
                    await discordUser.SendMessageAsync($"{events[i].Name} is starting soon, get ready gamer!");
                }
            }
        }
    }
}
