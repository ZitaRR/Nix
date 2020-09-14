using Nix.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nix.Controllers
{
    internal sealed class DiscordController : Controller
    {
        private readonly IDiscord discord;

        public DiscordController(IDiscord discord)
        {
            this.discord = discord;

            AppDomain.CurrentDomain.ProcessExit += OnExit;

            this.discord.StartAsync();
        }

        private void OnExit(object sender, EventArgs e)
            => discord.Dispose();
    }
}
