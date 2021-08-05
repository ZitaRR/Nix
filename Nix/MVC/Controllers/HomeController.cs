using Microsoft.Extensions.DependencyInjection;
using Nix.MVC.Views;
using System;

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
                    Options = new()
                    {
                        new() { Name = "Settings", View = SettingsController },
                        new() { Name = "Discord", View = DiscordController },
                        new() { Name = "Services", View = ServiceController },
                        new() { Name = "Exit", View = Exit },
                    },
                },
            };

            discord = services.GetService<DiscordController>();
            settings = services.GetService<SettingsController>();
            this.services = services.GetService<ServicesController>();

            Display();
        }

        public IView SettingsController()
        {
            return settings.Menu;
        }

        public IView DiscordController()
        {
            return discord.Menu;
        }

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
