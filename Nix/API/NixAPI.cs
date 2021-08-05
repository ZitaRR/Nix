using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Nix.Resources;
using Nix.Resources.Discord;
using Microsoft.AspNetCore.Builder;
using System.Security.Cryptography.X509Certificates;

namespace Nix.API
{
    public  class NixAPI
    {
        private readonly IDiscord discord;
        private readonly INixProvider nixProvider;
        private readonly AudioService audio;
        private readonly X509Certificate2 certificate;

        public NixAPI(IDiscord discord, INixProvider nixProvider, AudioService audio)
        {
            this.discord = discord;
            this.nixProvider = nixProvider;
            this.audio = audio;

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            certificate = store.Certificates[3];
        }

        public Task StartAsync()
        {
            Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .ConfigureWebHost(host =>
                {
                    host.UseKestrel(kestrel =>
                    {
                        kestrel.ListenAnyIP(5005, options =>
                        {
                            options.UseHttps(certificate);
                        });
                    });
                    host.ConfigureServices(services =>
                    {
                        services.AddSingleton(discord);
                        services.AddSingleton(nixProvider);
                        services.AddSingleton(audio);
                    });
                    host.UseStartup<Startup>();
                }).Build().Run();
            return Task.CompletedTask;
        }
    }
}
