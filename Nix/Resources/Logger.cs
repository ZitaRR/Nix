﻿using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nix.Resources
{
    internal class Logger : ILogger
    {
        public IList<LogMessage> Logs { get; private set; } = new List<LogMessage>();

        public void AppendLog(string source, string message, ConsoleColor colour)
            => Logs.Add(new LogMessage(source, message, colour));

        public void AppendLog(LogSeverity severity, string message)
        {
            ConsoleColor colour = ConsoleColor.Yellow;
            switch (severity)
            {
                case LogSeverity.Debug:
                    colour = ConsoleColor.Green;
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
            Console.WriteLine();
            foreach (var log in Logs)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"[{log.Date.ToShortTimeString()}]");
                Console.ForegroundColor = log.Colour;
                Console.Write($"[{log.Source.ToUpper()}] ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(log.Message);
            }

            Console.WriteLine("Press <Enter> to continue...");
            Console.ReadLine();
        }
    }
}