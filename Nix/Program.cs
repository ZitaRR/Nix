using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nix.Infrastructure;
using Nix.Infrastructure.Bot;
using Nix.Shared;
using System.Reflection;
using System.Threading.Tasks;

namespace Nix;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var version = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        _ = new AppInfo(version);

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddInfrastructure(config);

                services.AddHostedService(services => services.GetRequiredService<INixClient>());
            });

        await builder.RunConsoleAsync();
    }
}
