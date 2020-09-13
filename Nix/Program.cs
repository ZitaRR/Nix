using Nix.Controllers;
using Nix.Resources;
using System;
using System.Reflection;

namespace Nix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = Assembly.GetExecutingAssembly().GetName().Name;
            new HomeController();
        }
    }
}
