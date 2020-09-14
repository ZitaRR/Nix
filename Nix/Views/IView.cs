using Nix.Controllers;

namespace Nix.Views
{
    interface IView
    {
        Controller Controller { get; }
        string Name { get; }
        IView Parent { get; }
        void Display();
    }
}
