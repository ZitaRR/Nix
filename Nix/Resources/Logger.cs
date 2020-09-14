using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nix.Resources
{
    internal class Logger : ILogger
    {
        public IList<LogMessage> Logs { get; private set; } = new List<LogMessage>();

        public void AppendLog(string source, string message, ConsoleColor colour = ConsoleColor.White)
            => Logs.Add(new LogMessage(source, message, colour));

        public void AppendLog(LogSeverity severity, string message)
        {
            ConsoleColor colour = ConsoleColor.White;
            switch (severity)
            {
                case LogSeverity.Debug:
                    colour = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Warning:
                    colour = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    colour = ConsoleColor.DarkRed;
                    break;
            }
            AppendLog($"DISCORD/{severity}", message, colour);
        }

        public void AppendLog(Exception exception)
            => AppendLog("NIX/ERROR", exception.Message, ConsoleColor.DarkRed);

        public void WriteLogs()
        {
            foreach (var log in Logs)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{log.Date.ToShortTimeString()}]");
                Console.ForegroundColor = log.Colour;
                Console.Write($"[{log.Source.ToUpper()}] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(log.Message);
            }
        }
    }
}
