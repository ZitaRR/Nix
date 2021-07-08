using System;
using Nix.MVC;

namespace Nix.Resources
{
    internal class Option
    {
        public string Name { get; set; }
        public Func<IView> View { get; set; }
    }
}
