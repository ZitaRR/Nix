using Microsoft.Extensions.DependencyInjection;
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
            Extensions.AddServices(new ServiceCollection()).BuildServiceProvider().GetService<HomeController>();
            //new HomeController(Extensions.AddServices(new ServiceCollection()).BuildServiceProvider());
        }
    }

    static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection collection)
            => collection.AddSingleton<IDiscord, NixClient>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton(x => new HomeController(x))
                .AddSingleton<SettingsController>()
                .AddSingleton<DiscordController>();
    }
}
