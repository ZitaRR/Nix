using System;
using System.Collections.Generic;
using Discord;

namespace Nix.Resources
{
    interface ILogger
    {
        IList<LogMessage> Logs { get; }
        void AppendLog(string source, string message, ConsoleColor colour);
        void AppendLog(LogSeverity severity, string message);
        void AppendLog(Exception exception);
        void WriteLogs();
    }
}
