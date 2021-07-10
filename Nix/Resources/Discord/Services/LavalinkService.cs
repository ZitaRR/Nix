using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Nix.Resources.Discord
{
    public sealed class LavalinkService : ProcessService
    {
        public string Name { get; } = "Lavalink";
        public bool Interactive { get; } = false;
        public ILogger Logger { get; }

        private readonly Process lavalink;
        private readonly string directory = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;

        public LavalinkService(ILogger logger) : base(logger)
        {
            Logger = new Logger();

            lavalink = CreateProcess(
                Name,
                "run_lavalink.ps1",
                directory);

            AppDomain.CurrentDomain.ProcessExit += (e, s) =>
            {
                lavalink.Close();
            };

            lavalink.OutputDataReceived += OnOutput;
            lavalink.BeginOutputReadLine();
        }

        private void OnOutput(object sender, DataReceivedEventArgs args)
        {
            Logger.AppendLog(args.Data);
        }
    }
}
