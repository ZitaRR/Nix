using System;
using Nix.Resources;
using Nix.Views;

namespace Nix.Controllers
{
    internal abstract class Controller
    {
        public string Title { get; set; }
        public IView Menu { get; set; }
        public static IView CurrentView { get; set; }

        public Controller()
        {
            Title = GetType().Name.Replace(nameof(Controller), "");
            Config.Initialize();
        }

        internal void Display()
            => Menu.Display();
    }
}
