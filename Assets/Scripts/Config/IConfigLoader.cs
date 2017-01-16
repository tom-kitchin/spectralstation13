using System;

namespace Config
{
    interface IConfigLoader
    {
        string MapPath { get; }
        WorldConfig WorldConfig { get; }
    }
    
    public class ConfigLoadException : Exception
    {
        public ConfigLoadException () { }

        public ConfigLoadException (string message) : base(message) { }

        public ConfigLoadException (string message, Exception inner) : base(message, inner) { }
    }
}
