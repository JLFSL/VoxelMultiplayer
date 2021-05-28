using HarmonyLib;
using UnityEngine;

using System;

namespace VoxelMultiplayer.Game.Tools
{
	class ToolHelper
	{
		[HarmonyPatch(typeof(VoxelTycoon.Tools.ToolHelper), "Execute", new Type[] { typeof(double), typeof(VoxelTycoon.BudgetItem), typeof(Vector3), typeof(VoxelTycoon.Audio.Sound), typeof(VoxelTycoon.Audio.Sound)})]
		class Patch
		{
			static void Postfix(double price, VoxelTycoon.BudgetItem budgetItem, Vector3 position, VoxelTycoon.Audio.Sound successSound, VoxelTycoon.Audio.Sound failureSound)
			{
				Debug.Log("Patcher Postfix: ToolHelper.Execute 1");

				VoxelMultiplayer.Game.Tools.Builder.BuilderTool.checkBeforeSet = true;
				Debug.LogError(price + " " + budgetItem + " " + position + " " + VoxelMultiplayer.Game.Tools.Builder.BuilderTool.lastRecipe + " " + VoxelMultiplayer.Game.Tools.Builder.BuilderTool.lastRotation);
			}
		}

		[HarmonyPatch(typeof(VoxelTycoon.Tools.ToolHelper), "Execute", new Type[] { typeof(bool), typeof(double), typeof(VoxelTycoon.BudgetItem), typeof(Vector3), typeof(VoxelTycoon.Audio.Sound), typeof(VoxelTycoon.Audio.Sound) })]
		class Patch2
		{
			static void Postfix(bool predicate, double price, VoxelTycoon.BudgetItem budgetItem, Vector3 position, VoxelTycoon.Audio.Sound successSound, VoxelTycoon.Audio.Sound failureSound)
			{
				Debug.Log("Patcher Postfix: ToolHelper.Execute 2");

				VoxelMultiplayer.Game.Tools.Builder.BuilderTool.checkBeforeSet = true;
				VoxelTycoon.Buildings.BuildingRecipe lastRecipe = VoxelMultiplayer.Game.Tools.Builder.BuilderTool.lastRecipe;
				VoxelTycoon.BuildingRotation lastRotation = VoxelMultiplayer.Game.Tools.Builder.BuilderTool.lastRotation;

				Debug.LogError(price + " " + budgetItem + " " + position + " " + lastRecipe + " " + lastRotation + " "/* + lastRecipe.Building + " " + lastRecipe.Building.AssetId*/);
			}
		}
	}
}
