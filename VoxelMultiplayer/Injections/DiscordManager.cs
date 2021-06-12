using HarmonyLib;

using VoxelTycoon;
using VoxelTycoon.Integrations.Discord;

namespace VoxelMultiplayer.Injections
{
    [HarmonyPatch]
    class _DiscordManager
    {
        [HarmonyPatch(typeof(DiscordManager))]
        [HarmonyPatch("GetState")]
        static bool Prefix(ref string __result)
        {
            __result = "Playing Multiplayer - GV: " + BuildVersion.Current.ToShortestString();
            return false;
        }
    }
}
