using Figgle;
using Nix.Controllers;
using System;

namespace Nix.Views
{
    internal abstract class View : IView
    {
        public Controller Controller { get; private set; }
        public string Name { get; internal set; }
        public IView Parent { get; internal set; }

        private string asciiTitle;

        public View(Controller controller)
        {
            Controller = controller;
            asciiTitle = FiggleFonts.Standard.Render(Controller.Title);
        }

        public virtual void Display()
        {
            Controller.CurrentView = this;
            Console.Clear();
            Console.WriteLine(asciiTitle + "    " + Name);
        }
    }
}
