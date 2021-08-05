using System;

namespace Nix.MVC
{
    public sealed class NotificationView : View
    {
        public string Message { get; private set; }

        public NotificationView(string message)
        {
            Message = message;
        }

        public NotificationView(Controller controller, string message) : base(controller) 
        {
            Message = message;
        }

        public override void Display()
        {
            int height = Message.Split('\n').Length + 1;
            string[] lines = Message.Split('\n');
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                int length = lines[i].Length;
                if (length > width)
                {
                    width = length + 1;
                }
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write("╒");
            int centerOffset = (width - Name.Length) / 2;
            for (int i = 0; i < width - 1; i++)
            {
                if (i > centerOffset - 2 &&  i < width - centerOffset)
                {
                    Console.Write($" {Name} ");
                    i = width - centerOffset;
                }
                Console.Write("═");
            }
            Console.SetCursorPosition(width, 0);
            Console.WriteLine("╕");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(width, i);
                Console.WriteLine("║");
            }
            Console.SetCursorPosition(0, height);
            Console.Write("╘");
            for (int i = 0; i < width - 1; i++)
            {
                Console.Write("═");
            }
            Console.SetCursorPosition(width, height);
            Console.Write("╛");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine("║");
            }
            Console.SetCursorPosition(1, 1);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(1, i + 1);
                Console.WriteLine(lines[i]);
            }
            Console.ReadLine();
        }
    }
}
