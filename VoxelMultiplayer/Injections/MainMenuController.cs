using HarmonyLib;
using UnityEngine;

using VoxelTycoon.MainMenu;
using VoxelTycoon.UI;


namespace VoxelMultiplayer.Injections
{
    [HarmonyPatch]
    class _MainMenuController
    {
        [HarmonyPatch(typeof(MainMenuController))]
        [HarmonyPatch("SetupUI")]
        static bool Prefix()
        {
            Debug.Log("_MainMenuController: Showing multiplayer menu");
            UIManager.Current.CreateFrame<MPUI.MainMenu>(FrameAnchoring.Fullscreen).Show();
            Client.menuLoaded = true;
            _GameController.Playable = false;
            return false;
        }
    }
}
