using Microsoft.Extensions.DependencyInjection;
using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class HomeController : Controller
    {
        private readonly IServiceProvider services;

        public HomeController(IServiceProvider services)
        {
            this.services = services;

            Menu = new NavigationView(this)
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

            Display();
        }

        public IView SettingsController()
            => services.GetService<SettingsController>().Menu;

        public IView DiscordController()
            => services.GetService<DiscordController>().Menu;

        public IView Exit()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return null;
        }
    }
}
