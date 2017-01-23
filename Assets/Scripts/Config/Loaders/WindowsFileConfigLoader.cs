using System.IO;
using UnityEngine;
using Config.Parsers;
using Config.Loaders.Helpers;
using Config.Datatypes;

namespace Config.Loaders
{
    public class WindowsFileConfigLoader : IConfigLoader
    {
        string _mapName;
        IFilesystemConfigHelper _filesystemHelper;

        public WindowsFileConfigLoader(string mapName)
        {
            _mapName = mapName;
        }

        public WorldConfig Load(IConfigParser parser)
        {
            _filesystemHelper = new WindowsFilesystemConfigHelper(_mapName);
            
            if (!Directory.Exists(_filesystemHelper.ConfigDirectoryPath))
            {
                throw new ConfigLoadException("Map directory " + _filesystemHelper.ConfigDirectoryPath + " not found");
            }

            WorldConfig worldConfig;

            byte[] entityData = GetDataForFile(_filesystemHelper.EntityFilePath);
            byte[] layoutData = GetDataForFile(_filesystemHelper.LayoutFilePath);
                
            worldConfig = parser.Parse(entityData, layoutData);

            foreach (SpriteMap spriteMap in worldConfig.spriteMaps.Values)
            {
                if (spriteMap.filename != null && spriteMap.filename != "")
                {
                    spriteMap.texture = LoadTextureFromPath(_filesystemHelper.GetSpriteFilePath(spriteMap.filename));
                } else
                {
                    throw new ConfigLoadException("Sprite map loaded by filesystem has no path to load the sprite!");
                }
            }

            return worldConfig;
        }

        Texture2D LoadTextureFromPath (string path)
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(fileData);
            return texture;
        }

        byte[] GetDataForFile (string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ConfigLoadException("Critical config file for map not found at path " + filePath);
            }

            return File.ReadAllBytes(filePath);
        }
    }
}