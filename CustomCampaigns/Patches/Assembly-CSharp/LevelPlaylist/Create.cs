using HarmonyLib;
using Mod.CustomCampaigns.Scripts;

namespace Mod.CustomCampaigns.Patches
{
	internal static class LevelPlaylist__Create
	{
		[HarmonyPatch(typeof(LevelPlaylist), nameof(LevelPlaylist.Create), typeof(bool))]
		internal static class LevelPlaylist__Create_overload0
		{
			[HarmonyPostfix]
			internal static void Postfix(ref LevelPlaylist __result)
			{
				var playlistData = __result.gameObject.AddComponent<LevelPlaylistCompoundData>();

				playlistData.Playlist = __result;
			}
		}

		// Normally the below Create functions calls LevelPlaylist.Create(bool) underneath,
		// but still add the component in-case another mod changes that.

		[HarmonyPatch(typeof(LevelPlaylist), nameof(LevelPlaylist.Create), typeof(LevelSet), typeof(string))]
		internal static class LevelPlaylist__Create_overload1
		{
			[HarmonyPostfix]
			internal static void Postfix(ref LevelPlaylist __result)
			{
				var playlistData = __result.gameObject.GetOrAddComponent<LevelPlaylistCompoundData>();

				playlistData.Playlist = __result;
			}
		}

		[HarmonyPatch(typeof(LevelPlaylist), nameof(LevelPlaylist.Create), typeof(LevelSet), typeof(string), typeof(GameModeID))]
		internal static class LevelPlaylist__Create_overload2
		{
			[HarmonyPostfix]
			internal static void Postfix(ref LevelPlaylist __result, GameModeID customGameModeID)
			{
				var playlistData = __result.gameObject.GetOrAddComponent<LevelPlaylistCompoundData>();

				playlistData.Playlist = __result;
				playlistData.CustomGameModeID = customGameModeID;
			}
		}

		[HarmonyPatch(typeof(LevelPlaylist), nameof(LevelPlaylist.Create), typeof(LevelSet), typeof(string), typeof(LevelGroupFlags))]
		internal static class LevelPlaylist__Create_overload3
		{
			[HarmonyPostfix]
			internal static void Postfix(ref LevelPlaylist __result, LevelGroupFlags flags)
			{
				var playlistData = __result.gameObject.GetOrAddComponent<LevelPlaylistCompoundData>();

				playlistData.Playlist = __result;
				playlistData.LevelGroupFlags = flags;
			}
		}
	}
}
