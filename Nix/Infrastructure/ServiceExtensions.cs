using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nix.Infrastructure;

internal static class ServiceExtensions
{
    public static NixConfig CreateNixConfig(this IConfigurationBuilder builder)
    {
        var config = builder
            .AddEnvironmentVariables()
            .Build();
        return new(config[NixConstants.DISCORD_TOKEN]);
    }
}
