using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class DiscordController : Controller
    {
        private readonly NixClient nix;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;
        private LogView log;
        private NavigationView guilds;
        private NavigationView guild;
        private NavigationView channels;
        private NavigationView users;

        public DiscordController(NixClient nix, ILogger logger, IPersistentStorage storage)
        {
            this.nix = nix;
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

            _ = this.nix.StartAsync();
        }

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
            throw new NotImplementedException();
        }

        private IView Guild(ulong guildId)
        {
            throw new NotImplementedException();
        }

        private IView Channels(ulong guildId)
        {
            throw new NotImplementedException();
        }

        private IView Users(ulong guildId)
        {
            throw new NotImplementedException();
        }

        private IView Events(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
