using HarmonyLib;
using Mod.CustomCampaigns.Scripts;

namespace Mod.CustomCampaigns.Patches
{
	[HarmonyPatch(typeof(LevelPlaylist), nameof(LevelPlaylist.Load))]
	internal static class LevelPlaylist__Load
	{
		[HarmonyPostfix]
		internal static void Postfix(ref UnityEngine.GameObject __result, string levelPlaylistPath)
		{
			var playlistData = __result.gameObject.AddComponent<LevelPlaylistCompoundData>();
			playlistData.FilePath = levelPlaylistPath;
			playlistData.Playlist = __result.GetComponent<LevelPlaylist>();
		}
	}
}
