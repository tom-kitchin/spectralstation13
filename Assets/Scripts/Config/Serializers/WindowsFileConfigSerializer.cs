using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using Config.Filesystem.Helpers;

namespace Config.Serializers
{
    public class WindowsFileConfigSerializer : IConfigSerializer
    {
        IFilesystemConfigHelper _filesystemHelper;
        string _mapName;

        public WindowsFileConfigSerializer (string mapName)
        {
            _mapName = mapName;
            _filesystemHelper = new WindowsFilesystemConfigHelper(mapName);
        }

        public byte[] Serialize ()
        {
            SerializedConfig serial = new SerializedConfig();
            serial.mapName = _mapName;
            serial.spriteDatas = SerializeSpriteFiles();
            serial.entityData = SerializeEntityFile();

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, serial);
                return stream.ToArray();
            }
        }

        Dictionary<string, byte[]> SerializeSpriteFiles ()
        {
            Dictionary<string, byte[]> serials = new Dictionary<string, byte[]>();

            foreach (string spriteFilePath in _filesystemHelper.GetSpriteFilePaths())
            {
                serials.Add(Path.GetFileNameWithoutExtension(spriteFilePath), LoadDataFromFile(spriteFilePath));
            }

            return serials;
        }

        byte[] SerializeEntityFile ()
        {
            return LoadDataFromFile(_filesystemHelper.EntityFilePath);
        }

        byte[] LoadDataFromFile (string filepath)
        {
            return File.ReadAllBytes(filepath);
        }

        /**
         * Compresses the data loaded, but not used yet because premature optimisation etc etc.
         */
        byte[] LoadCompressedDataFromFile (string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (DeflateStream compressStream = new DeflateStream(ms, CompressionMode.Compress))
                    {
                        byte[] buffer = new byte[81920];
                        int count;
                        while ((count = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}


//    serializedEntityTypes = new Dictionary<string, byte[]>();

//            BinaryFormatter formatter = new BinaryFormatter();
//    SHA256 checksumHasher = SHA256Managed.Create();

//    byte[] entityTypesData;
//            using (MemoryStream stream = new MemoryStream())
//            {
//                EntityTypeDataDictionary entityTypes = _config.entityTypes;

//    formatter.Serialize(stream, entityTypes);
//                entityTypesData = stream.ToArray();
//            }
//byte[] checksum = checksumHasher.ComputeHash(entityTypesData);
//serializedSpriteMaps.Add(BitConverter.ToString(checksum), entityTypesData);

