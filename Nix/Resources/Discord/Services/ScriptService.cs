using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            scriptPath = @"..\..\..\scripts\";
        }

        public async Task<string> RunScript(string script, bool redirect = true)
        {
            var process = Process.Start(new ProcessStartInfo("cmd.exe", 
                $"/C powershell.exe {scriptPath}{script}")
            {
                RedirectStandardOutput = redirect,
                WorkingDirectory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName
            });

            logger.AppendLog($"Running script: {script}");
            return await process.StandardOutput.ReadLineAsync();
        }
    }
}
