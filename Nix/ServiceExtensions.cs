using Microsoft.Extensions.Configuration;

namespace Nix;

internal static class ServiceExtensions
{
    internal static NixConfig CreateNixConfig(this IConfigurationBuilder builder)
    {
        var config = builder
            .AddEnvironmentVariables()
            .Build();
        return new(config[NixConstants.DISCORD_TOKEN]);
    }
}
