using System;
using System.Collections.Generic;
using System.Text;

namespace Nix.Resources
{
    internal sealed class LogMessage
    {
        public DateTime Date { get; private set; }
        public string Source { get; private set; }
        public string Message { get; private set; }
        public ConsoleColor Colour { get; private set; }

        public LogMessage(string source, string message, ConsoleColor colour)
        {
            Date = DateTime.UtcNow;
            Source = source;
            Message = message;
            Colour = colour;
        }
    }
}
