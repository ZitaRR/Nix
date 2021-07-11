using Nix.Resources;
using System;
using System.Collections.Generic;

namespace Nix.MVC.Views
{
    public sealed class Navigation : IBehaviour
    {
        public IView View { get; private set; }

        public List<Option> Options { get; set; }
        public int Index { get; private set; } = 0;

        public Navigation(int index = 0)
        {
            Index = index;
        }

        private void ListOptions()
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (i == Index)
                {
                    SetFocus(i, true);
                }
                else SetFocus(i, false);
            }
        }

        private void SetFocus(int index, bool value)
        {
            Console.SetCursorPosition(0, index + MVC.View.OFFSET);
            var option = Options[index];

            if (!value)
            {
                string whitespace = "";
                for (int i = 0; i < Config.Data.SelectionMarker.Length; i++)
                {
                    whitespace += " ";
                }
                Console.WriteLine(option.Name + whitespace);
            }
            else
            {
                Console.ForegroundColor = (ConsoleColor)Config.Data.FontColour;
                Console.BackgroundColor = (ConsoleColor)Config.Data.BackgroundColour;
                Console.WriteLine(option.Name + Config.Data.SelectionMarker);
            }

            Console.BackgroundColor = default;
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void Input()
        {
            do
            {
                ListOptions();
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (Index > 0)
                            Index--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (Index < Options.Count - 1)
                            Index++;
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        try { View.Parent.Display(); }
                        catch { break; }
                        return;
                    case ConsoleKey.Enter:
                        Options[Index].View().Display();
                        Options[Index].View().Parent.Display();
                        return;
                }
            } while (true);
        }

        public void Start(IView view)
        {
            View = view;
            Input();
        }
    }
}
