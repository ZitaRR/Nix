﻿using System;

namespace Nix.MVC.Views
{
    public sealed class TextInput : IBehaviour
    {
        public IView View { get; private set; }
        public Action<TextInput> Action { get; set; }
        public string Prompt { get; set; }
        public string UserInput { get; set; }
        public bool HideInput { get; set; }
        public bool Repeat { get; set; }

        private bool repeat;

        private void Input()
        {
            Console.Write(Prompt + UserInput);

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
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
            repeat = Repeat;

            do
            {
                Input();
                Console.WriteLine();
                Action(this);
            } while (repeat);
        }

        public void Cancel()
        {
            repeat = false;
        }
    }
}
