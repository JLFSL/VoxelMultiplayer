using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game.Buildings
{
    class Building
    {
        public static bool isGameLoaded = false;

        [HarmonyPatch(typeof(VoxelTycoon.Buildings.Building), "Build")]
        class Patch
        {
            static void Postfix(VoxelTycoon.Buildings.Building __instance, VoxelTycoon.Xyz position, VoxelTycoon.Buildings.BuildStrategy buildStrategy = null)
            {
                if (isGameLoaded) // prevent a trillion new game building logs
                {
                    if (!__instance.Parent)
                    {
                        Debug.LogError("Patcher Postfix: Building.Build" + __instance.Parent + " " + __instance.IsAsset + " " + __instance.AssetId + " " + __instance.DisplayName + " " + __instance.isActiveAndEnabled + " " + __instance.IsBuilt + " " + __instance.IsDestroyed + " " + __instance.Position + __instance.Rotation);

                       // VoxelTycoon.Buildings.Building test = __instance;
                       // test.IsBuilt = false;

                        //InvokeMethod(test, "Restore", new VoxelTycoon.Xyz(test.Position.X + 15, test.Position.Y + 15, test.Position.Z), __instance.Id+1);

                        //test.Build(test.Position);
                    } 
                    else
                    {
                        Debug.Log("Patcher Postfix: Building.Build Child" + __instance.Parent + " " + __instance.IsAsset + " " + __instance.AssetId + " " + __instance.DisplayName + " " + __instance.isActiveAndEnabled + " " + __instance.IsBuilt + " " + __instance.IsDestroyed + " " + __instance.Position + __instance.Rotation);
                    }
                }
            }
        }
    }
}
