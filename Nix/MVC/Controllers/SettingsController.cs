using Nix.Resources;
using System.Collections.Generic;
using System;
using Nix.MVC.Views;

namespace Nix.MVC
{
    public sealed class SettingsController : Controller
    {
        private View fontColours;
        private View backgroundColours;
        private View markerInput;
        private readonly Array colours;

        public SettingsController()
        {
            colours = Enum.GetValues(typeof(ConsoleColor));

            Menu = new View(this)
            {
                Name = "Settings",
                Parent = CurrentView,
                Behaviour = new Navigation
                {
                    Options = new()
                    {
                        new() { Name = "Change Font Colour", View = ChangeFontColour },
                        new() { Name = "Change Background Colour", View = ChangeBackgroundColour },
                        new() { Name = "Change Selection Marker", View = ChangeSelectionMarker }
                    }
                }
            };
        }

        public IView ChangeSelectionMarker()
        {
            if (markerInput is null)
            {
                markerInput = new(this)
                {
                    Name = "Selection Marker",
                    Parent = CurrentView,
                    Behaviour = new TextInput
                    {
                        Prompt = "Enter Selection Marker",
                        UserInput = Config.Data.SelectionMarker,
                        Action = input =>
                        {
                            Config.Data.SelectionMarker = input.UserInput;
                            Config.Save();
                        }
                    }
                };
            }
            return markerInput;
        }

        public IView ChangeFontColour()
        {
            if (fontColours is null)
            {
                List<Option> options = new();
                foreach (ConsoleColor colour in colours)
                {
                    options.Add(new()
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

                fontColours = new(this)
                {
                    Name = "Change Font Colour",
                    Parent = Menu,
                    Behaviour = new Navigation
                    {
                        Options = options,
                    },
                };
            }
            return fontColours;
        }

        public IView ChangeBackgroundColour()
        {
            if (backgroundColours is null)
            {
                List<Option> options = new();
                foreach (ConsoleColor colour in colours)
                {
                    options.Add(new()
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

                backgroundColours = new(this)
                {
                    Name = "Change Background Colour",
                    Parent = CurrentView,
                    Behaviour = new Navigation
                    {
                        Options = options
                    }
                };
            }
            return backgroundColours;
        }
    }
}
