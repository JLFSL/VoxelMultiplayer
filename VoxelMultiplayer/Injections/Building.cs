using HarmonyLib;
using LiteNetLib;
using UnityEngine;

using VoxelMultiplayer.Network;
using VoxelMultiplayer.Network.Packets;

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
            BuildingData data = new BuildingData()
            {
                AssetId = __instance.AssetId,
                Rotation = (int)__instance.Rotation,
                PositionX = __instance.Position.X,
                PositionY = __instance.Position.Y,
                PositionZ = __instance.Position.Z,
                DisplayName = __instance.DisplayName
            };

            if (__instance.Parent != null)
                data.ParentId = __instance.Parent.Id;

            if (_GameController.Playable)
            {
                if (Client.ClientPeer)
                {
                    Debug.LogError("Packet " + data.GetType() + " from Client");
                    ClientPeer.Manager.SendToAll(ClientPeer.Processor.Write(data), DeliveryMethod.ReliableOrdered, ClientPeer.Manager.GetEnumerator().Current);
                }
                else if (Client.ServerPeer)
                {
                    Debug.LogError("Packet " + data.GetType() + " from Server");
                    ServerPeer.Manager.SendToAll(ServerPeer.Processor.Write(data), DeliveryMethod.ReliableOrdered, ServerPeer.Manager.GetEnumerator().Current);
                }
            }
        }
    }
}
