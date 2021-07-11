using Figgle;
using System;

namespace Nix.MVC
{
    public abstract class View : IView
    {
        public const int OFFSET = 8;

        public Controller Controller { get; private set; }
        public string Name { get; set; }
        public IView Parent { get; set; }

        private readonly string asciiTitle;

        public View() { }

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
