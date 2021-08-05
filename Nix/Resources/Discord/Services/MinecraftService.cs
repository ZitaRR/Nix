using System.IO;
using System.Reflection;

namespace Nix.Resources.Discord
{
    public sealed class MinecraftService : ProcessServiceBase
    {
        private readonly string directory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

        public MinecraftService(ILogger logger) : base(logger) 
        {
            //CreateProcess("run.bat", directory);
        }
    }
}
