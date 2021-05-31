using HarmonyLib;
using UnityEngine;

using System;

namespace VoxelMultiplayer.Game.Tools.Builder
{
    class BuilderToolTrackHandler
    {
        [HarmonyPatch(typeof(VoxelTycoon.Tools.Builder.BuilderToolTrackHandler<VoxelTycoon.Buildings.BuildingRecipeTrack, VoxelTycoon.Tracks.Track>), "BuildGhost", new Type[] { typeof(VoxelTycoon.Buildings.Building) })]
        class Patch
        {
            static void Prefix(VoxelTycoon.Tools.Builder.BuilderToolTrackHandler<VoxelTycoon.Buildings.BuildingRecipeTrack, VoxelTycoon.Tracks.Track> __instance, VoxelTycoon.Buildings.Building parent)
            {
                if (parent.IsBuilt)
                    Debug.LogError("Patcher Postfix: BuilderToolTrackHandler.BuildGhost" + parent.AssetId + parent.DisplayName + parent.Position);
            }
        }

        /*[HarmonyPatch(typeof(VoxelTycoon.Tools.Builder.BuilderToolTrackHandler<VoxelTycoon.Buildings.BuildingRecipeTrack, VoxelTycoon.Tracks.Track>), "PlaceGhost", new Type[] { typeof(VoxelTycoon.Buildings.Building) })]
        class Patch2
        {
            static void Prefix(VoxelTycoon.Tools.Builder.BuilderToolTrackHandler<VoxelTycoon.Buildings.BuildingRecipeTrack, VoxelTycoon.Tracks.Track> __instance, VoxelTycoon.Buildings.Building parent)
            {
                if (parent.IsBuilt)
                    Debug.LogError("Patcher Postfix: BuilderToolTrackHandler.PlaceGhost");
            }
        }*/
    }
}
