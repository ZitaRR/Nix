using System;

namespace Nix.MVC
{
    public class Option
    {
        public string Name { get; set; }
        public Func<IView> View { get; set; }
    }
}
