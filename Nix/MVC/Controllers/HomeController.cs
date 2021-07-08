using Microsoft.Extensions.DependencyInjection;
using Nix.Resources;
using System;
using System.Collections.Generic;

namespace Nix.MVC
{
    internal sealed class HomeController : Controller
    {
        private readonly DiscordController discord;
        private readonly SettingsController settings;

        public HomeController(IServiceProvider services)
        {
            CurrentView = Menu = new NavigationView(this)
            {
                Name = "Main Menu",
                Parent = null,
                Options = new List<Option>
                {
                    new Option { Name = "Settings", View = SettingsController },
                    new Option { Name = "Discord", View = DiscordController },
                    new Option { Name = "Exit", View = Exit }
                },
            };

            discord = services.GetService<DiscordController>();
            settings = services.GetService<SettingsController>();

            Display();
        }

        public IView SettingsController()
            => settings.Menu;

        public IView DiscordController()
            => discord.Menu;

        public IView Exit()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return null;
        }
    }
}
