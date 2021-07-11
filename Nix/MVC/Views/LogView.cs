using Nix.Resources;
using System;

namespace Nix.MVC
{
    public sealed class LogView : View
    {
        private readonly ILogger logger;

        public LogView(Controller controller, ILogger logger) : base(controller) 
        {
            this.logger = logger;
            this.logger.OnLog += Logger_OnLog;
        }

        private void Logger_OnLog(NixLogMessage log)
        {
            if (!Active)
                return;

            logger.WriteLog(log);
        }

        public override void Display()
        {
            base.Display();
            logger.WriteLogs();

            if (Behaviour is null)
            {
                Console.ReadLine();
                return;
            }

            Behaviour.Start(this);
        }
    }
}
