using System;
using Nix.Views;

namespace Nix.Resources
{
    internal class Option
    {
        public string Name { get; set; }
        public Func<IView> View { get; set; }
    }
}
