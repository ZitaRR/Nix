using Figgle;
using Nix.Controllers;
using System;

namespace Nix.Views
{
    internal abstract class View : IView
    {
        public delegate void ChangeView(IView view);
        public static event ChangeView OnViewChange;

        public Controller Controller { get; }
        public string Name { get; internal set; }
        public IView Parent { get; internal set; }

        private readonly string asciiTitle;

        public View(Controller controller)
        {
            Controller = controller;
            asciiTitle = FiggleFonts.Standard.Render(Controller.Title);
        }

        public virtual IView Display()
        {
            OnViewChange?.Invoke(this);
            Console.WriteLine(asciiTitle + "                   Made by Zita");
            return this;
        }
    }
}
