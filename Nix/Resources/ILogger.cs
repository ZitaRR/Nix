using System;
using System.Collections.Generic;
using Discord;
using LiteDB;

namespace Nix.Resources
{
    public interface ILogger
    {
        IList<NixLogMessage> Logs { get; }
        void AppendLog(string source, string message, ConsoleColor colour = ConsoleColor.Magenta);
        void AppendLog(string message, LogSeverity severity = LogSeverity.Info);
        void AppendLog(Exception exception);
        void AppendLog(LiteException exception);
        void WriteLog(NixLogMessage log);
        void WriteLogs();
    }
}
