using System;
using System.IO;
using Newtonsoft.Json;

namespace Nix.Resources
{
    public static class Config
    {
        internal static ConfigData Data { get; set; } = new ConfigData();

        private static readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\config.cfg";
        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized)
                return;

            if (!File.Exists(path))
            {
                Save();
                Console.WriteLine($"Config was created at [{path}]");
                Console.ReadLine();
            }
            else
            {
                Load();
                Console.WriteLine("Config was intialized.");
                Console.ReadLine();
            }

            initialized = true;
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static void Load()
        {
            if (!ConfigExists())
                Save();
            var json = File.ReadAllText(path);
            Data = JsonConvert.DeserializeObject<ConfigData>(json);
        }

        private static bool ConfigExists()
            => File.Exists(path);

        internal class ConfigData
        {
            public string SelectionMarker { get; set; } = " <";
            public int FontColour { get; set; } = 1;
            public int BackgroundColour { get; set; } = 1;
        }
    }
}
