namespace Nix.MVC
{
    internal abstract class Controller
    {
        public string Title { get; set; }
        public IView Menu { get; set; }
        public static IView CurrentView { get; set; }

        public Controller()
        {
            Title = GetType().Name.Replace(nameof(Controller), "");
        }

        internal void Display()
            => Menu.Display();
    }
}
