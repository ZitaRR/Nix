using System.IO;
using System.Reflection;

namespace Nix.Resources.Discord
{
    public sealed class LavalinkService : ProcessServiceBase
    {
        private readonly string directory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

        public LavalinkService(ILogger logger) : base(logger)
        {
            CreateProcess("run_lavalink.ps1", directory);
        }
    }
}
