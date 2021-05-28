using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game
{
    class Company
    {
        [HarmonyPatch(typeof(VoxelTycoon.Company), "AddMoney")]
        class Patch
        {
            static void Prefix(VoxelTycoon.Company __instance, double value, VoxelTycoon.BudgetItem budgetItem, bool important = true)
            {
                Debug.Log("Patcher Prefix: AddMoney");
            }

            static void Postfix(VoxelTycoon.Company __instance, double value, VoxelTycoon.BudgetItem budgetItem, bool important = true)
            {
                Debug.Log("Patcher Postfix: AddMoney");
            }
        }
    }
}
