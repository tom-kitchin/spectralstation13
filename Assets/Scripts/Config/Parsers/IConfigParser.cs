using System;
using System.IO;

namespace Config.Parsers
{
    public interface IConfigParser
    {
        WorldConfig Parse (byte[] entityData);
        WorldConfig Parse (byte[] entityData, byte[] layoutData);
    }

    public class ConfigParseException : Exception
    {
        public ConfigParseException () { }

        public ConfigParseException (string message) : base(message) { }

        public ConfigParseException (string message, Exception inner) : base(message, inner) { }
    }
}
