using Discord;
using System;
using System.Collections.Generic;

namespace Nix.Resources
{
    public class Logger : ILogger
    {
        public event ILogger.Log OnLog;

        public List<NixLogMessage> Logs { get; private set; } = new List<NixLogMessage>();

        public void AppendLog(string source, string message, ConsoleColor colour)
        {
            NixLogMessage log = new(source, message, colour);
            Logs.Add(log);
            OnLog?.Invoke(log);
        }

        public void AppendLog(string message, LogSeverity severity)
        {
            ConsoleColor colour = ConsoleColor.Magenta;
            switch (severity)
            {
                case LogSeverity.Debug:
                    colour = ConsoleColor.Green;
                    break;
                case LogSeverity.Warning:
                    colour = ConsoleColor.DarkYellow;
                    break;
                case LogSeverity.Error:
                    colour = ConsoleColor.DarkRed;
                    break;
            }
            AppendLog($"DISCORD", message, colour);
        }

        public void AppendLog(Exception exception)
        {
            AppendLog("CLIENT", exception.Message, ConsoleColor.DarkRed);
        }

        public void WriteLog(NixLogMessage log)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{log.Date.ToShortTimeString()}]");
            Console.ForegroundColor = log.Colour;

            string whitespace = "";
            int offset = 12 - log.Source.Length;
            for (int i = 0; i < offset; i++)
            {
                whitespace += " ";
            }

            Console.Write($"[{log.Source.ToUpper()}]" + whitespace);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log.Message);
        }

        public void WriteLogs()
        {
            Console.WriteLine();
            int count = Logs.Count;
            for (int i = 0; i <  count; i++)
            {
                WriteLog(Logs[i]);
            }
        }
    }
}
