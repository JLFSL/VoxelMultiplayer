using HarmonyLib;
using UnityEngine;

namespace VoxelMultiplayer.Game.Tools.Builder
{
    class BuilderTool
    {
		public static bool checkBeforeSet = false;

		public static VoxelTycoon.BuildingRotation lastRotation;
		public static VoxelTycoon.Buildings.BuildingRecipe lastRecipe;

		[HarmonyPatch(typeof(VoxelTycoon.Tools.Builder.BuilderTool), "OnUpdate")]
		class Patch
		{
			static void Prefix()
			{
				//Debug.Log("Patcher Prefix: BuilderTool.OnUpdate");
			}

			static void Postfix(VoxelTycoon.Tools.Builder.BuilderTool __instance)
			{
				if (VoxelTycoon.InputHelper.WorldMouseDown && checkBeforeSet)
				{
					lastRotation = __instance.Rotation;
					lastRecipe = __instance.Recipe;

					Debug.Log("Patcher Postfix: BuilderTool.OnUpdate WorldMouseDown - " + lastRotation + " " + lastRecipe);

					checkBeforeSet = false;
				}
			}
		}
	}
}
