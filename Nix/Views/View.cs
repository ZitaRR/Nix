using Figgle;
using Nix.Controllers;
using System;

namespace Nix.Views
{
    internal abstract class View : IView
    {
        public delegate void ChangeView(IView view);
        public static event ChangeView OnViewChange;

        public string Name { get; internal set; }
        public IView Parent { get; internal set; }

        public virtual void Display()
            => OnViewChange?.Invoke(this);
    }
}
