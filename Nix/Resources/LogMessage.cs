using System;

namespace Nix.Resources
{
    internal sealed class NixLogMessage
    {
        public DateTime Date { get; private set; }
        public string Source { get; private set; }
        public string Message { get; private set; }
        public ConsoleColor Colour { get; private set; }

        public NixLogMessage(string source, string message, ConsoleColor colour)
        {
            Date = DateTime.Now;
            Source = source;
            Message = message;
            Colour = colour;
        }
    }
}
