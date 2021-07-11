namespace Nix.MVC
{
    public abstract class Controller
    {
        public string Title { get; set; }
        public IView Menu { get; set; }
        public static IView CurrentView { get; set; }

        public Controller()
        {
            Title = GetType().Name.Replace(nameof(Controller), "");
        }

        public void Display()
            => Menu.Display();
    }
}
