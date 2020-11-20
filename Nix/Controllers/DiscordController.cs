using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class DiscordController : Controller
    {
        private readonly IDiscord discord;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;
        private LogView log;
        private NavigationView guilds;
        private NavigationView guild;
        private NavigationView channels;
        private NavigationView users;

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
            var _guilds = storage.FindAll<NixGuild>();
            var options = new List<Option>();
            foreach (var guild in _guilds)
            {
                options.Add(new Option
                {
                    Name = guild.Name,
                    View = () => Guild(guild.GuildID)
                });
            }

            int index = guilds?.Index ?? 0;

            return guilds = new NavigationView(this, index)
            {
                Name = "Guilds",
                Parent = Menu,
                Options = options
            };
        }

        private IView Guild(ulong guildId)
        {
            var _guild = storage.FindOne<NixGuild>(x => x.GuildID == guildId);
            guild = new NavigationView(this)
            {
                Name = _guild.Name ?? "N/A",
                Parent = Guilds(),
                Options = new List<Option>
                    {
                        new Option { Name = "Users", View = () => Users(guildId) },
                        new Option { Name = "Channels", View = () => Channels(guildId) }
                    }
            };
            return guild;
        }

        private IView Channels(ulong guildId)
        {
            var _channels = storage.Find<NixChannel>(x => x.GuildID == guildId);
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
                            Parent = Channels(guildId)
                        };
                    }
                });
            }

            int index = channels?.Index ?? 0;

            return channels = new NavigationView(this, index)
            {
                Name = "Channels",
                Parent = Guild(guildId),
                Options = options
            };
        }

        private IView Users(ulong guildId)
        {
            var _users = storage.Find<NixUser>(x => x.GuildID == guildId);
            var options = new List<Option>();
            foreach (var user in _users)
            {
                options.Add(new Option
                {
                    Name = user.Name,
                    View = () =>
                    {
                        return new NotificationView(this, user.ToString())
                        {
                            Name = user.Name,
                            Parent = Users(guildId)
                        };
                    }
                });
            }

            int index = users?.Index ?? 0;

            return users = new NavigationView(this, index)
            {
                Name = "Users",
                Parent = Guild(guildId),
                Options = options
            };
        }
    }
}
