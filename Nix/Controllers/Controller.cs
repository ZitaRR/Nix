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
            View.OnViewChange += View_OnViewChange;
            Config.Initialize();
        }

        internal void Display()
            => Menu.Display();

        private void View_OnViewChange(IView view)
        {
            CurrentView = view;
            Console.Clear();
            var ascii = Figgle.FiggleFonts.Standard.Render(Title);
            Console.WriteLine(ascii + "     " + CurrentView.Name);
        }
    }
}
