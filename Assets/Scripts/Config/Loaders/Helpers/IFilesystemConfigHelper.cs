﻿namespace Config.Filesystem.Helpers
{
    public interface IFilesystemConfigHelper
    {
        string ConfigDirectoryPath { get; }
        string MapCacheDirectoryPath { get; }
        string SpritesDirectoryPath { get; }
        string EntityFilePath { get; }
        string LayoutFilePath { get; }

        string GetSpriteFilePath (string filename);
        string[] GetSpriteFilePaths ();
    }
}
