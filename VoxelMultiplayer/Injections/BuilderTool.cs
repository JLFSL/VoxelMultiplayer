using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

using VoxelTycoon;
using VoxelTycoon.Audio;
using VoxelTycoon.Buildings;
using VoxelTycoon.Tools;
using VoxelTycoon.Tools.Builder;
using VoxelTycoon.UI;

namespace VoxelMultiplayer.Injections
{
    /*[HarmonyPatch]
    class _BuilderTool
    {
		static bool _deactivate = false;

        [HarmonyPatch(typeof(BuilderTool))]
        [HarmonyPatch("OnUpdate")]
		static void Prefix(BuilderTool __instance)
        {
			
        }

		/*static bool Prefix(ref bool __result, BuilderTool __instance)
		{
			bool flag = (Utility.Utils.GetProperty<bool>(__instance, "MultipleModeSupported") && LazyManager<InputManager>.Current.GetKey(LazyManager<Settings>.Current.MultipleModeKey));
			if (_deactivate && !flag)
			{
				__result = true;
			}
			Utility.Utils.Invoke(__instance, "InvalidateGhost");

			if (Utility.Utils.GetProperty2<Building>(__instance, "Ghost", typeof(Building)) == null)
			{
				Utility.Utils.Invoke(__instance, "CreateGhost", __instance.Rotation);
				Utility.Utils.Invoke(__instance, "SetDefaultTint");
			}
			if (ToolHelper.IsHotkeyDown(LazyManager<Settings>.Current.RotateKey))
				Utility.Utils.Invoke(__instance, "Rotate");

			if (ToolHelper.IsHotkeyDown(LazyManager<Settings>.Current.DecreaseBuildingHeightKey))
				Utility.Utils.SetField(__instance, "Height", Utility.Utils.GetProperty<int>(__instance, "Height") - 1);

			if (ToolHelper.IsHotkeyDown(LazyManager<Settings>.Current.IncreaseBuildingHeightKey))
				Utility.Utils.SetField(__instance, "Height", Utility.Utils.GetProperty<int>(__instance, "Height") + 1);

			Utility.Utils.Invoke(__instance, "ResetTint");
			Utility.Utils.Invoke(__instance, "PlaceGhost", (Xyz)Utility.Utils.Invoke(__instance, "GetPosition"));
			Utility.Utils.Invoke(__instance, "InvalidatePositions");

			try
			{
				Utility.Utils.Invoke(__instance, "GetWhiteList", Utility.Utils.GetField(__instance, "_tempWhiteList"));
				Utility.Utils.Invoke(__instance, "InvalidateCanRemove", Utility.Utils.GetField(__instance, "_tempWhiteList"));
				Utility.Utils.Invoke(__instance, "InvalidateCanFlatten", Utility.Utils.GetField(__instance, "_tempWhiteList"));
			}
			finally
			{
				Utility.Utils.Invoke(Utility.Utils.GetField(__instance, "_tempWhiteList"), "Clear");
			}

			bool canBuild = (bool)Utility.Utils.Invoke(__instance, "CanBuild");
			double price = (double)Utility.Utils.Invoke(__instance, "GetPrice") + Utility.Utils.GetProperty<double>(__instance, "_removePrice") + Utility.Utils.GetProperty<double>(__instance, "_flattenPrice");
			Vector3 hintPosition = (Vector3)Utility.Utils.Invoke(__instance, "GetHintPosition");
			bool flag2 = (bool)Utility.Utils.Invoke(__instance, "ValidateGhost", canBuild, price, hintPosition);

			Utility.Utils.Invoke(__instance, "OnValidated", flag2);
			Utility.Utils.Invoke(__instance, "SetTint", flag2);
			Utility.Utils.Invoke(__instance, "Select", flag2);
			Utility.Utils.Invoke(__instance, "InvalidateGrid");
			Utility.Utils.Invoke(__instance, "InvalidateCursor", flag2);
			if (InputHelper.WorldMouseDown && ToolHelper.Execute(flag2, price, BudgetItem.Buildings, hintPosition, Utility.Utils.GetProperty<Sound>(Utility.Utils.GetProperty(__instance, "Ghost"), "BuildSound"), R.Audio.BuildBlocked.Value))
			{
				Utility.Utils.Invoke(__instance, "OnBuildingGhost");
				Utility.Utils.Invoke(__instance, "ResetTint");
				Utility.Utils.Invoke(__instance, "ResetDefaultTint");
				Utility.Utils.Invoke(__instance, "RemoveObstacles");
				Utility.Utils.Invoke(__instance, "Flatten", false);
				Utility.Utils.Invoke(__instance, "BuildGhost");

				Utility.Utils.SetField(__instance, "Ghost", null);
				if (!flag)
				{
					__result = true;
				}
				_deactivate = true;
			}
			__result = false;

			return false;
		}*/
	//}
}