using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Config.Serializers
{
    public class ConfigDeserializer : IConfigDeserializer
    {
        public SerializedConfig Deserialize (byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (SerializedConfig)formatter.Deserialize(ms);
            }
        }

        /**
         * We're not actually compressing data so we don't need this yet, but might come in handy.
         */
        MemoryStream DecompressData (byte[] data)
        {
            using (MemoryStream input = new MemoryStream(data, false))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (DeflateStream decompressStream = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        byte[] buffer = new byte[81920];
                        int count;
                        while ((count = decompressStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            output.Write(buffer, 0, count);
                        }
                    }
                    return output;
                }
            }
        }
    }
}