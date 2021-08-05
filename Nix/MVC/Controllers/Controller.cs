namespace Nix.MVC
{
    public abstract class Controller
    {
        private static IView view;
        public static IView CurrentView
        {
            get { return view; }
            set
            {
                if (view != null)
                {
                    view.Active = false;
                }

                view = value;
                view.Active = true;
            }
        }

        public string Title { get; set; }
        public IView Menu { get; set; }

        public Controller()
        {
            Title = GetType().Name.Replace(nameof(Controller), "");
        }

        public void Display()
        {
            Menu.Display();
        }
    }
}
