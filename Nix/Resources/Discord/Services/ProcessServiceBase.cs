using System.Diagnostics;

namespace Nix.Resources.Discord
{
    public abstract class ProcessServiceBase
    {
        private const string SCRIPT_PATH = @"..\..\..\scripts\";

        private readonly ILogger logger;

        public ProcessServiceBase(ILogger logger)
        {
            this.logger = logger;
        }

        protected Process CreateProcess(string processName, string script, string directory, bool interactive = false)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"{SCRIPT_PATH}{script}",
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardInput = interactive,
            });

            logger.AppendLog("PROCESS", $"Started [{processName}]");
            return process;
        }
    }
}
