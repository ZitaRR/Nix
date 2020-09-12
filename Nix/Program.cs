using Nix.Presenters;
using System;
using System.Drawing;
using System.Reflection;

namespace Nix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;
            new HomePresenter();
        }
    }
}
