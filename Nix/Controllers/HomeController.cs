using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class HomeController : Controller
    {
        private Controller settings;

        public HomeController()
        {
            Menu = new NavigationView
            {
                Name = "Main Menu",
                Parent = null,
                Options = new List<Option>
                {
                    new Option { Name = "Settings", View = SettingsController },
                    new Option { Name = "Exit", View = Exit }
                },
            };

            Display();
        }

        public IView SettingsController()
        {
            if (settings is null)
                settings = new SettingsController();
            return settings.Menu;
        }

        public IView Exit()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return null;
        }
    }
}
