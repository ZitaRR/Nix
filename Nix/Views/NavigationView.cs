using Nix.Controllers;
using Nix.Resources;
using System;
using System.Collections.Generic;

namespace Nix.Views
{
    internal sealed class NavigationView : View
    {
        public List<Option> Options { get; internal set; }
        public int Index { get; private set; } = 0;

        private const int offset = 8;

        public NavigationView(Controller controller) : base(controller) { }

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
            Console.SetCursorPosition(0, index + offset);
            var option = Options[index];

            if (!value)
            {
                Console.WriteLine(option.Name + "  ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(option.Name + " <");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void Input()
        {
            do
            {
                ListOptions();
                var key = Console.ReadKey(false).Key;
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
                    case ConsoleKey.Backspace:
                        try { Parent.Display(); }
                        catch { break; }
                        return;
                    case ConsoleKey.Enter:
                        Options[Index].View().Display();
                        return;
                }
            } while (true);
        }

        public override IView Display()
        {
            base.Display();
            Input();
            return this;
        }
    }
}
