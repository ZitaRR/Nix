using System.Diagnostics;

namespace Nix.Resources.Discord
{
    public sealed class ProcessService
    {
        private const string SCRIPT_PATH = @"..\..\..\scripts\";

        private readonly ILogger logger;

        public ProcessService(ILogger logger)
        {
            this.logger = logger;
        }

        public Process CreateProcess(string processName, string script, string directory, bool interactive = false)
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
