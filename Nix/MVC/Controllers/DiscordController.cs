using Nix.MVC.Views;
using Nix.Resources;
using System;

namespace Nix.MVC
{
    public sealed class DiscordController : Controller
    {
        private readonly NixClient nix;
        private readonly ILogger logger;
        private readonly IPersistentStorage storage;
        private LogView log;
        private View guilds;
        private View guild;
        private View channels;
        private View users;

        public DiscordController(NixClient nix, ILogger logger, IPersistentStorage storage)
        {
            this.nix = nix;
            this.logger = logger;
            this.storage = storage;

            Menu = new View(this)
            {
                Name = "Discord",
                Parent = CurrentView,
                Behaviour = new Navigation
                {
                    Options = new()
                    {
                        new() { Name = "Logs", View = Logs },
                        new() { Name = "Guilds", View = Guilds },
                    },
                },
            };

            _ = this.nix.StartAsync();
        }

        private IView Logs()
        {
            if (log is null)
            {
                log = new(this, logger)
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
