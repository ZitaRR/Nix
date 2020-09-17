using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nix.Controllers
{
    internal sealed class DiscordController : Controller
    {
        private LogView log;
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
    }
}
