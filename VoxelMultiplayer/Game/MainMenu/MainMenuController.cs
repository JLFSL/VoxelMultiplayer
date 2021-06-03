using HarmonyLib;
using UnityEngine;

using System.Collections;

namespace VoxelMultiplayer.Game.MainMenu
{
    class MainMenuController
    {
        [HarmonyPatch(typeof(VoxelTycoon.MainMenu.MainMenuController), "SetupUI")]
        class Bootstrap
        {
            static bool Prefix(VoxelTycoon.MainMenu.MainMenuController __instance)
            {
                Debug.Log("Patcher Prefix: SetupUI");
                VoxelMultiplayer.Client.GameReady = false;
                Debug.Log("Patcher SetupUI: Showing multiplayer menu");
                VoxelTycoon.UI.UIManager.Current.CreateFrame<VoxelMultiplayer.Game.MultiplayerMenu.UI.MultiplayerMenuFrame>(VoxelTycoon.UI.FrameAnchoring.Fullscreen).Show();
                VoxelMultiplayer.Client.menuLoaded = true;
                return false;
            }
        }
    }
}
