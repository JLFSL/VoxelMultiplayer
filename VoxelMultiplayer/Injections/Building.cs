using HarmonyLib;
using LiteNetLib;
using UnityEngine;

using VoxelMultiplayer.Network;
using VoxelMultiplayer.Network.Packets;

using VoxelTycoon;
using VoxelTycoon.Buildings;

namespace VoxelMultiplayer.Injections
{
    [HarmonyPatch]
    class _Building
    {
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("Build")]
        static void Postfix(Building __instance)
        {
            if(_GameController.Playable)
            {
                Server.Manager.SendToAll(Server.Processor.Write(new BuildingData()
                {
                    AssetId = __instance.AssetId,
                    Rotation = (int)__instance.Rotation,
                    PositionX = __instance.Position.X,
                    PositionY = __instance.Position.Y,
                    PositionZ = __instance.Position.Z
                }), DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
