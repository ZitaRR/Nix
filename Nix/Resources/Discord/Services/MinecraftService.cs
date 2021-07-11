using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Nix.Resources.Discord
{
    public sealed class MinecraftService : ProcessServiceBase
    {
        public string Name { get; } = "Minecraft";
        public bool Interactive { get; } = true;
        public ILogger Logger { get; }

        private readonly Process minecraft;
        private readonly string directory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

        public MinecraftService(ILogger logger) : base(logger)
        {
            Logger = new Logger();

            minecraft = CreateProcess(
                Name,
                "run.bat",
                directory,
                Interactive);

            AppDomain.CurrentDomain.ProcessExit += (e, s) =>
            {
                minecraft.Close();
            };

            minecraft.OutputDataReceived += OnOutput;
            minecraft.BeginOutputReadLine();
        }

        private void OnOutput(object sender, DataReceivedEventArgs e)
        {
            Logger.AppendLog(e.Data);
        }

        public void Write(string message)
        {
            Logger.AppendLog(message);

            minecraft.StandardInput.WriteLine(message);
            minecraft.StandardInput.Flush();
        }
    }
}
