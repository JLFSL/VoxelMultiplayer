using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game.Tools.TrackBuilder
{
    public  class TrackBuilderTool
    {
        [HarmonyPatch(typeof(VoxelTycoon.Tools.TrackBuilder.TrackBuilderTool<VoxelTycoon.Tracks.Track, VoxelTycoon.Tracks.TrackBridge, VoxelTycoon.Tracks.TrackTunnel, VoxelTycoon.Tools.Direction4>), "OnGhostBuild")]
        class Patch
        {
            static void Postfix(VoxelTycoon.Buildings.Building __instance)
            {
                Debug.LogError("Patcher Postfix: BuilderTool.BuildGhost " + __instance.ToString());
            }
        }
    }
}
