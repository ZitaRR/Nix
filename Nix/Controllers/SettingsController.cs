using Nix.Resources;
using Nix.Views;
using System.Collections.Generic;
using System;

namespace Nix.Controllers
{
    internal sealed class SettingsController : Controller
    {
        private NavigationView fontColours;
        private NavigationView backgroundColours;
        private readonly Array colours;

        public SettingsController()
        {
            colours = Enum.GetValues(typeof(ConsoleColor));

            Menu = new NavigationView(this)
            {
                Name = "Settings",
                Parent = CurrentView,
                Options = new List<Option>
                {
                    new Option { Name = "Change Font Colour", View = ChangeFontColour },
                    new Option { Name = "Change Background Colour", View = ChangeBackgroundColour }
                }
            };
        }

        public IView ChangeSelectionMarker()
        {
            throw new NotImplementedException();
        }

        public IView ChangeFontColour()
        {
            if (fontColours is null)
            {
                var options = new List<Option>();
                foreach (var colour in colours)
                {
                    options.Add(new Option
                    {
                        Name = colour.ToString(),
                        View = () =>
                        {
                            Config.Data.FontColour = (int)colour;
                            Config.Save();
                            return CurrentView.Parent;
                        }
                    });
                }

                fontColours = new NavigationView(this)
                {
                    Name = "Change Font Colour",
                    Parent = Menu,
                    Options = options
                };
            }
            return fontColours;
        }

        public IView ChangeBackgroundColour()
        {
            if (backgroundColours is null)
            {
                var options = new List<Option>();
                foreach (var colour in colours)
                {
                    options.Add(new Option
                    {
                        Name = colour.ToString(),
                        View = () =>
                        {
                            Config.Data.BackgroundColour = (int)colour;
                            Config.Save();
                            return CurrentView.Parent;
                        }
                    });
                }

                backgroundColours = new NavigationView(this)
                {
                    Name = "Change Background Colour",
                    Parent = CurrentView,
                    Options = options
                };
            }
            return backgroundColours;
        }
    }
}
