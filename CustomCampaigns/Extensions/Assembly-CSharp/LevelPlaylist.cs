#pragma warning disable RCS1110
using Mod.CustomCampaigns.Scripts;
using System;
using System.IO;

namespace Mod.CustomCampaigns.Extensions
{
    public static class LevelPlaylistExtensions
    {
        public static bool IsResourcesPlaylist(this LevelPlaylist playlist)
        {
            //Mod.Log.LogInfo("   I'm gonna check it!");
            LevelPlaylistCompoundData playlistData = playlist.GetComponent<LevelPlaylistCompoundData>();
            if (!playlistData || playlistData.FilePath == null)
            {
                return true;
            }
            string path = new FileInfo(playlistData.FilePath).FullName.UniformPathSeparators();
            //Mod.Log.LogInfo("   " + path);
            string resourcesPath = new DirectoryInfo(Path.Combine(UnityEngine.Application.dataPath, "Resources")).FullName.UniformPathSeparatorsTrimmed() + "/";
            return path.StartsWith(resourcesPath, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string ResourcePath(this LevelPlaylist playlist)
        {
            LevelPlaylistCompoundData playlistData = playlist.GetComponent<LevelPlaylistCompoundData>();
            if (!playlistData || playlistData.FilePath == null)
            {
                return string.Empty;
            }
            string path = new FileInfo(playlistData.FilePath).FullName.UniformPathSeparators();
            string resourcesPath = new DirectoryInfo(Path.Combine(UnityEngine.Application.dataPath, "Resources")).FullName.UniformPathSeparatorsTrimmed() + "/";

            return resourcesPath;
        }
    }
}
