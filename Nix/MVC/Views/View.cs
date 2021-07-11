using Figgle;
using Nix.MVC.Views;
using System;

namespace Nix.MVC
{
    public class View : IView
    {
        public const int OFFSET = 8;

        public Controller Controller { get; private set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public IView Parent { get; set; }
        public IBehaviour Behaviour { get; set; }

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
            Console.SetCursorPosition(0, OFFSET);

            if (GetType() == typeof(View))
                Behaviour?.Start(this);
        }
    }
}
