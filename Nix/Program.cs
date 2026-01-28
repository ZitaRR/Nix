using System;

namespace Nix;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("Input: ");
            string input = Console.ReadLine();
            Console.WriteLine($"You wrote: {input}");
        }
    }
}
