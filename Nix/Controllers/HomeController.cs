using Nix.Resources;
using Nix.Views;
using System;
using System.Collections.Generic;

namespace Nix.Controllers
{
    internal sealed class HomeController : Controller
    {
        private NavigationView menu;
        private NavigationView settings;

        public HomeController()
        {
            menu = new NavigationView(this)
            {
                Name = "Main Menu",
                Parent = null,
                Options = new List<Option>
                {
                    new Option { Name = "Settings", View = Settings },
                    new Option { Name = "Exit", View = Exit }
                },
            };
            menu.Display();
        }

        public IView Settings()
        {
            if (settings is null)
            {
                settings = new NavigationView(this)
                {
                    Name = "Settings",
                    Parent = menu,
                    Options = new List<Option>
                    {
                        new Option { Name = "Settings, Menu 1", View = menu.Display },
                        new Option { Name = "Settings, Menu 2", View = menu.Display }
                    }
                };
            }
            return settings;
        }

        public IView Exit()
        {
            Console.WriteLine("Exiting...");
            Environment.Exit(0);
            return null;
        }
    }
}
