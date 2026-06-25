using Mod.CustomCampaigns.Extensions;
using UnityEngine;

namespace Mod.CustomCampaigns.Scripts
{
	public class CreateCampaignLogic : MonoBehaviour
	{
		private LevelGridGrid grid_;

		private void Awake()
		{
			grid_ = GetComponentInParent<LevelGridGrid>();
			if (!grid_)
			{
				Mod.Log.LogError(nameof(LevelGridGrid) + " component not found");
			}
			Mod.Log.LogInfo("Created Component!");
		}

		private void Update()
		{
			if (!grid_.isGridPushed_)
			{
				return;
			}

			bool isArcade = grid_.levelGridMenu_.displayType_ == LevelSelectMenuAbstract.DisplayType.Arcade;

			if (isArcade && !grid_.playlist_.IsResourcesPlaylist() && G.Sys.InputManager_.GetKeyUp(InputAction.MenuSpecial_3))
			{
				//Mod.Log.LogInfo("Pressed the Menu Special Button");
				LevelSet set = new LevelSet();
				set.resourcesLevelNameAndPathPairsInSet_ = grid_.playlist_.GetLevelSet();
				//foreach(LevelNameAndPathPair LNP in set.resourcesLevelNameAndPathPairsInSet_) { Mod.Log.LogInfo("Name: " + LNP.levelName_ + " Path: " + LNP.levelPath_); }

				grid_.levelGridMenu_.menuPanelManager_.ShowOkCancel($"Create a campaign out of the [c][9480e7]{grid_.playlist_.playlistName_}[-][c] playlist?", "Create Campaign",
					new MessagePanelLogic.OnButtonClicked(() =>
					{
						LevelPlaylist playlist = LevelPlaylist.Create(set, grid_.playlist_.playlistName_, GameModeID.Nexus);
						playlist.Awake();
						//Mod.Log.LogInfo("Playlist to save name: " + playlist.playlistName_);
						playlist.Save();
						grid_.levelGridMenu_.buttonList_.Remove(grid_.levelGridMenu_.selectedEntry_);
						grid_.levelGridMenu_.buttonList_.SortAndUpdateVisibleButtons();
						grid_.levelGridMenu_.SelectEntry(grid_.levelGridMenu_.ScrollableEntries_[0], true);
					}));
            }
		}
	}
}
