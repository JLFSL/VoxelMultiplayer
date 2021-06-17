using System;

using HarmonyLib;
using LiteNetLib;
using UnityEngine;

using VoxelTycoon;
using VoxelTycoon.Audio;
using VoxelTycoon.Tools;

namespace VoxelMultiplayer.Injections
{
    [HarmonyPatch]
    class _ToolHelper
    {
        [HarmonyPatch(typeof(ToolHelper))]
        [HarmonyPatch("Execute")]
        [HarmonyPatch(new Type[] { typeof(bool), typeof(double), typeof(BudgetItem), typeof(Vector3), typeof(Sound), typeof(Sound) } )]
        static void Prefix(bool predicate, double price, BudgetItem budgetItem, Vector3 position, Sound successSound, Sound failureSound)
        {
            Network.Server.Manager.SendToAll(Network.Server.Processor.Write(new Network.Packets.ToolExecuteData()
            {
                Predicate = predicate,
                Price = price,
                BudgetItem = (int)budgetItem,

                PositionX = position.x,
                PositionY = position.y,
                PositionZ = position.z
            }), DeliveryMethod.ReliableOrdered);
        }
    }
}
