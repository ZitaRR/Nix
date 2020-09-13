using Nix.Controllers;

namespace Nix.Views
{
    interface IView
    {
        string Name { get; }
        IView Parent { get; }
        void Display();
    }
}
