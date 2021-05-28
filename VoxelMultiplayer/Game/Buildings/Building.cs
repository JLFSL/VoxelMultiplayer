using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game.Buildings
{
    class Building
    {
        [HarmonyPatch(typeof(VoxelTycoon.Buildings.Building), "OnPlaced")]
        class Patch
        {
            static void Postfix()
            {
                //Debug.Log("Patcher Postfix: Building.OnPlaced");
            }
        }
    }
}
