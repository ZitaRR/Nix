using Microsoft.Extensions.DependencyInjection;
using Nix.Controllers;
using Nix.Resources;
using System;

namespace Nix
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = $"{Utility.Name} v{Utility.Version}";

            Config.Initialize();

            new ServiceCollection()
                .AddNixServices()
                .BuildServiceProvider()
                .GetService<HomeController>();
        }
    }
}
