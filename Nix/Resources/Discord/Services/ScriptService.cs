using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Nix.Resources.Discord
{
    public sealed class ScriptService
    {
        private readonly ILogger logger;
        private readonly string scriptPath;

        public ScriptService(ILogger logger)
        {
            this.logger = logger;
            scriptPath = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent}\\Scripts\\";
        }

        public void RunScript(string script, out string result, bool redirectOutput = true)
        {
            var process = Process.Start(new ProcessStartInfo("powershell.exe", scriptPath + script)
            {
                RedirectStandardOutput = true
            });

            process.Start();
            if (redirectOutput)
            {
                result = "N/A";
                process.OutputDataReceived += (s, e) => logger.AppendLog(e.Data);
                process.BeginOutputReadLine();
            }
            else
            {
                result = process.StandardOutput.ReadLine();
            }

            logger.AppendLog($"Running script: {script}");
        }
    }
}
