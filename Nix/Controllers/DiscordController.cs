using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class DiscordController : Controller
    {
        private LogView log;
        private NavigationView guilds;
        private NavigationView channels;
        private readonly IDiscord discord;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;

        public DiscordController(IDiscord discord, ILogger logger, IPersistentStorage storage)
        {
            this.discord = discord;
            this.logger = logger;
            this.storage = storage;

            Menu = new NavigationView(this)
            {
                Name = "Discord",
                Parent = CurrentView,
                Options = new List<Option>
                {
                    new Option { Name = "Logs", View = Logs },
                    new Option { Name = "Guilds", View = Guilds }
                }
            };

            AppDomain.CurrentDomain.ProcessExit += OnExit;

            this.discord.StartAsync();
        }

        private void OnExit(object sender, EventArgs e)
            => discord.Dispose();

        private IView Logs()
        {
            if (log is null)
            {
                log = new LogView(this, logger)
                {
                    Name = "Logs",
                    Parent = CurrentView,
                };
            }
            return log;
        }

        private IView Guilds()
        {
            if (guilds is null)
            {
                var _guilds = storage.FindAll<NixGuild>();
                var options = new List<Option>();
                foreach (var guild in _guilds)
                {
                    options.Add(new Option
                    {
                        Name = guild.Name,
                        View = () =>
                        {
                            return Channels(guild.GuildID);
                        }
                    });
                }

                guilds = new NavigationView(this)
                {
                    Name = "Guilds",
                    Parent = CurrentView,
                    Options = options
                };
            }
            return guilds;
        }

        private IView Channels(ulong guildID)
        {
            var _channels = storage.Find<NixChannel>(x => x.GuildID == guildID);
            var options = new List<Option>();
            foreach (var channel in _channels)
            {
                options.Add(new Option
                {
                    Name = channel.Name,
                    View = () =>
                    {
                        return new NotificationView(this, channel.ToString())
                        {
                            Name = channel.Name,
                            Parent = Channels(guildID)
                        };
                    }
                });
            }

            int index = channels?.Index ?? 0;

            return channels = new NavigationView(this, index)
            {
                Name = "Channels",
                Parent = Guilds(),
                Options = options
            };
        }
    }
}
