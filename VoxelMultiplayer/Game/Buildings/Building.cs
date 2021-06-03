using HarmonyLib;
using LiteNetLib;

using UnityEngine;

namespace VoxelMultiplayer.Game.Buildings
{
    class Building
    {

        [HarmonyPatch(typeof(VoxelTycoon.Buildings.Building), "Build")]
        class Patch
        {
            static void Postfix(VoxelTycoon.Buildings.Building __instance, VoxelTycoon.Xyz position, VoxelTycoon.Buildings.BuildStrategy buildStrategy = null)
            {
                if (VoxelMultiplayer.Client.GameReady) // prevent a trillion new game building logs
                {
                    if (!__instance.Parent)
                    {
                        Debug.LogError("Patcher Postfix: Building.Build " + __instance.name + " " + __instance.IsAsset + " " + __instance.AssetId + " " + __instance.DisplayName + " " + __instance.isActiveAndEnabled + " " + __instance.IsBuilt + " " + __instance.IsDestroyed + " " + __instance.Position + __instance.Rotation);

                        Network.Server.Processor.Send(Network.Server._clientPeer, new Network.Packets.BuildingData()
                        {
                            AssetId = __instance.AssetId, 
                            Rotation = (int)__instance.Rotation,
                            PositionX = __instance.Position.X,
                            PositionY = __instance.Position.Y,
                            PositionZ = __instance.Position.Z,
                        }, DeliveryMethod.ReliableOrdered);
                    } 
                    else
                    {
                        Debug.Log("Patcher Postfix: Building.Build Child " + __instance.Parent + " " + __instance.IsAsset + " " + __instance.AssetId + " " + __instance.DisplayName + " " + __instance.isActiveAndEnabled + " " + __instance.IsBuilt + " " + __instance.IsDestroyed + " " + __instance.Position + __instance.Rotation);
                    }
                }
            }
        }
    }
}
