using System;
using Config.Parsers;

namespace Config.Loaders
{
    interface IConfigLoader
    {
        WorldConfig Load (IConfigParser parser);
    }

    public class ConfigLoadException : Exception
    {
        public ConfigLoadException () { }

        public ConfigLoadException (string message) : base(message) { }

        public ConfigLoadException (string message, Exception inner) : base(message, inner) { }
    }
}
