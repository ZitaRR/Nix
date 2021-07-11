using System;

namespace Nix.MVC.Views
{
    public sealed class TextInput : IBehaviour
    {
        public IView View { get; private set; }
        public Action<TextInput> Callback { get; set; }
        public string Prompt { get; set; }
        public string UserInput { get; set; }
        public bool HideInput { get; set; }

        private void Input()
        {
            Console.SetCursorPosition(0, MVC.View.OFFSET);
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

        public void Start(IView view)
        {
            View = view;
            Input();
            Callback(this);
        }
    }
}
