using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Nix.Resources
{
    public static class Utility
    {
        public static string Version
        {
            get
            {
                return FileVersionInfo.GetVersionInfo(
                    Assembly.GetExecutingAssembly().Location).ProductVersion;
            }
        }

        public static string Name
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static string ConnectionString
        {
            get
            {
                string name;
#if DEBUG
                name = "DevNixDB";
#else
                name = "NixDB";
#endif
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
        }
    }
}
