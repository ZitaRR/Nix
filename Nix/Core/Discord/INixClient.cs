using Microsoft.Extensions.Hosting;
using System;

namespace Nix.Core.Discord;

public interface INixClient : IHostedService, IAsyncDisposable
{

}
