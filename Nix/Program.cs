using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix;

class Program
{
    internal readonly static string Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

    static async Task Main(string[] args)
    {
        Console.Title = $"Nix ({Version})";

        var config = new ConfigurationBuilder()
            .CreateNixConfig();

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<DiscordSocketClient>(_ => new(new DiscordSocketConfig
                {
                    GatewayIntents = Discord.GatewayIntents.All
                }));
                services.AddSingleton<CommandService>();
                services.AddSingleton<CommandHandler>();

                services.AddSingleton(typeof(NixConfig), config);
                services.AddHostedService<NixClient>();
            });

        await builder.RunConsoleAsync();
    }
}
