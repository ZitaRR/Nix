using Microsoft.Extensions.Hosting;
using System;

namespace Nix.Infrastructure.Bot;

public interface INixClient : IHostedService, IAsyncDisposable
{

}
