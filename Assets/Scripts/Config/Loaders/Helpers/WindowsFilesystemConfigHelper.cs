using System;
using System.IO;

namespace Config.Loaders.Helpers
{
    public class WindowsFilesystemConfigHelper : IFilesystemConfigHelper
    {
        string _mapName;
        string _mapCacheDirectoryPath;
        string _configDirectoryPath;
        string _spritesDirectoryPath;
        string _entityFilePath;
        string _layoutFilePath;

        public WindowsFilesystemConfigHelper (string mapName)
        {
            _mapName = mapName;
        }

        public string ConfigDirectoryPath {
            get {
                if (_configDirectoryPath == null)
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string mapsPath = Path.Combine(Path.Combine(documentsPath, "Spectral Station 13"), "Maps");
                    _configDirectoryPath = Path.Combine(mapsPath, _mapName);
                }
                return _configDirectoryPath;
            }
        }

        public string MapCacheDirectoryPath {
            get {
                if (_mapCacheDirectoryPath == null)
                {
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    _mapCacheDirectoryPath = Path.Combine(Path.Combine(documentsPath, "Spectral Station 13"), "mapcache");
                }
                return _mapCacheDirectoryPath;
            }
        }

        public string SpritesDirectoryPath {
            get {
                if (_spritesDirectoryPath == null)
                {
                    _spritesDirectoryPath = Path.Combine(ConfigDirectoryPath, "sprites");
                }
                return _spritesDirectoryPath;
            }
        }

        public string EntityFilePath {
            get {
                if (_entityFilePath == null)
                {
                    _entityFilePath = Path.Combine(ConfigDirectoryPath, "entityData.json");
                }
                return _entityFilePath;
            }
        }

        public string LayoutFilePath {
            get {
                if (_layoutFilePath == null)
                {
                    _layoutFilePath = Path.Combine(ConfigDirectoryPath, "layoutData.json");
                }
                return _layoutFilePath;
            }
        }

        public string GetSpriteFilePath (string filename)
        {
            return Path.Combine(SpritesDirectoryPath, filename);
        }

        public string[] GetSpriteFilePaths ()
        {
            return Directory.GetFiles(SpritesDirectoryPath);
        }
        
        public string GetMapCacheFilePath (string filename)
        {
            return Path.Combine(MapCacheDirectoryPath, filename);
        }

        public string[] GetMapCacheFilePaths ()
        {
            return Directory.GetFiles(MapCacheDirectoryPath);
        }
    }
}
