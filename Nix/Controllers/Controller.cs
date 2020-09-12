using Nix.Views;

namespace Nix.Controllers
{
    internal abstract class Controller
    {
        public string Title { get; set; }
        public IView CurrentView { get; set; }

        public Controller()
        {
            Title = GetType().Name.Replace(nameof(Controller), "");
            View.OnViewChange += View_OnViewChange;
        }

        private void View_OnViewChange(IView view)
        {
            CurrentView = view;
            System.Console.Clear();
        }
    }
}
