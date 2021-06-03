using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game.Integrations.Discord
{
    class DiscordManager
    {
        [HarmonyPatch(typeof(VoxelTycoon.Integrations.Discord.DiscordManager), "GetState")]
        class Patch
        {
            static bool Prefix(ref string __result)
            {
                Debug.Log("Patcher Prefix: GetState");

                __result = "Playing Multiplayer - GV: " + VoxelTycoon.BuildVersion.Current.ToShortestString();
                return false;
            }
        }
    }
}
