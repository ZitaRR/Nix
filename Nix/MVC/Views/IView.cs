using Nix.MVC.Views;

namespace Nix.MVC
{
    public interface IView
    {
        Controller Controller { get; }
        string Name { get; }
        IView Parent { get; }
        IBehaviour Behaviour { get; }
        void Display();
    }
}
