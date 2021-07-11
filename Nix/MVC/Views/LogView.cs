using Nix.MVC.Views;
using Nix.Resources;
using System;

namespace Nix.MVC
{
    public sealed class LogView : View
    {
        public bool Interactive
        {
            get
            {
                return Behaviour is TextInput || !(Behaviour is null);
            }
        }

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

            if (!Interactive)
            {
                Console.ReadLine();
                return;
            }

            Behaviour.Start(this);
        }
    }
}
