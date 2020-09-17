using Nix.Controllers;
using Nix.Resources;
using System;

namespace Nix.Views
{
    internal sealed class LogView : View
    {
        private readonly ILogger logger;

        public LogView(Controller controller, ILogger logger) : base(controller) 
        {
            this.logger = logger;
            Logger.OnLog += Logger_OnLog;
        }

        private void Logger_OnLog(NixLogMessage log)
        {
            logger.WriteLog(log);
        }

        public override void Display()
        {
            base.Display();
            Console.SetCursorPosition(0, OFFSET);
            logger.WriteLogs();
        }
    }
}
