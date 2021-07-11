using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public abstract class ProcessServiceBase
    {
        private const string SCRIPT_PATH = @"..\..\..\scripts\";

        public string Name { get; protected set; }
        public ILogger Logger { get; private set; }

        private readonly ILogger logger;
        protected Process process;
        protected StreamWriter input;

        public ProcessServiceBase(ILogger logger)
        {
            this.logger = logger;

            Name = GetType().Name.Replace("Service", "");
            Logger = new Logger();

            AppDomain.CurrentDomain.ProcessExit += OnExit;
        }

        protected void CreateProcess(string script, string directory)
        {
            process = Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"{SCRIPT_PATH}{script}",
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
            });

            logger.AppendLog("PROCESS", $"Started [{Name}]");

            input = process.StandardInput;
            process.OutputDataReceived += OnOutput;
            process.BeginOutputReadLine();
        }

        protected void OnOutput(object sender, DataReceivedEventArgs e)
        {
            Logger.AppendLog(e.Data);
        }

        private void OnExit(object sender, EventArgs e)
        {
            process.CancelOutputRead();
            process.Kill();
        }

        public async Task Write(string message)
        {
            await input.WriteLineAsync(message);
            await input.FlushAsync();
        }
    }
}
