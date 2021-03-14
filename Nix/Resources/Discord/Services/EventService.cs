using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Nix.Models;

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
            //timer.Start();
        }

        public async Task CreateEvent(ITextChannel channel, IMessage message, string name, string description, DateTime start)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateEvent(ITextChannel channel, SocketReaction react)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteEvent(ITextChannel channel, int id)
        {
            throw new NotImplementedException();
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
