using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Nix.MVC;

namespace Nix.Resources
{
    public static class Config
    {
        public static ConfigData Data { get; private set; } = new ConfigData();

        private static readonly string directory;
        private static readonly string path;
        private static bool initialized = false;

        static Config()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            directory = Directory.GetParent(assembly).Parent.Parent.FullName;
#if DEBUG
            path = $"{directory}/devconfig.json";
#else 
            path = $"{directory}/config.json";
#endif
        }

        public static void Initialize()
        {
            if (initialized)
                return;

            if (!File.Exists(path))
            {
                Save();
                new NotificationView($"Config was created at [{path}]")
                {
                    Name = "Config"
                }.Display();
            }
            else
            {
                Load();
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

        public class ConfigData
        {
            public string Token { get; set; }
            public string Prefix { get; set; } = ".";
            public string SelectionMarker { get; set; } = " <";
            public string SpotifyId { get; set; }
            public string SpotifySecret { get; set; }
            public int FontColour { get; set; } = 0;
            public int BackgroundColour { get; set; } = 15;
        }
    }
}
