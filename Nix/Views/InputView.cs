using Nix.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nix.Views
{
    internal sealed class InputView : View
    {
        public Action<InputView> Callback { get; set; }
        public string Prompt { get; }
        public string UserInput { get; private set; }
        public bool HideInput { get; }

        public InputView(Controller controller, string prompt, string oldInput = null, bool hide = false) : base(controller)
        {
            Prompt = prompt;
            HideInput = hide;
            UserInput = oldInput;
        }

        public override void Display()
        {
            base.Display();
            Console.SetCursorPosition(0, OFFSET);
            Input();
            Callback(this);
        }

        private void Input()
        {
            Console.Write(Prompt + UserInput);
            do
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        return;
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Backspace:
                        if (UserInput.Length > 0)
                        {
                            UserInput = UserInput[..^1];
                            Console.Write("\b \b");
                        }
                        break;
                    default:
                        UserInput += key.KeyChar;
                        Console.Write(key.KeyChar);
                        break;
                }
            } while (true);
        }
    }
}
