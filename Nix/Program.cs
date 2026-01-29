using System;

namespace Nix;

class Program
{
    static void Main(string[] args)
    {
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("DISCORD_TOKEN environment variable not found.");
            return;
        }

        Console.WriteLine("DISCORD_TOKEN: " + token);
    }
}
