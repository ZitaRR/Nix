using Microsoft.Extensions.DependencyInjection;
using Nix.MVC.Views;
using Nix.Resources;
using System;
using System.Collections.Generic;

namespace Nix.MVC
{
    public sealed class HomeController : Controller
    {
        private readonly DiscordController discord;
        private readonly SettingsController settings;
        private readonly ServicesController services;

        public HomeController(IServiceProvider services)
        {
            CurrentView = Menu = new View(this)
            {
                Name = "Main Menu",
                Parent = null,
                Behaviour = new Navigation
                {
                    Options = new List<Option>
                    {
                        new Option { Name = "Settings", View = SettingsController },
                        new Option { Name = "Discord", View = DiscordController },
                        new Option { Name = "Services", View = ServiceController },
                        new Option { Name = "Exit", View = Exit },
                    },
                },
            };

            discord = services.GetService<DiscordController>();
            settings = services.GetService<SettingsController>();
            this.services = services.GetService<ServicesController>();

            Display();
        }

        public IView SettingsController()
            => settings.Menu;

        public IView DiscordController()
            => discord.Menu;

        public IView ServiceController()
        {
            return services.Menu;
        }

        public IView Exit()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return null;
        }
    }
}
