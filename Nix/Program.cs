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
    private static readonly bool debug =
#if DEBUG
        true;
#else
        false;
#endif

    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var version = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version;

        var hash = Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        _ = new AppInfo(debug, $"{version.Major}.{version.Minor}.{version.Build}", hash);

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddInfrastructure(config);

                services.AddHostedService(services => services.GetRequiredService<INixClient>());
            });

        await builder.RunConsoleAsync();
    }
}
