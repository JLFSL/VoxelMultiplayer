using HarmonyLib;
using UnityEngine;

using System.Collections;

namespace VoxelMultiplayer.Game.Game
{
	class GameController
	{
		[HarmonyPatch(typeof(VoxelTycoon.Game.GameController), "Bootstrap")]
		class Bootstrap
		{
			static void Prefix(VoxelTycoon.Game.GameController __instance)
			{
				Debug.Log("Patcher Prefix: Bootstrap");
			}

			static void Postfix(VoxelTycoon.Game.GameController __instance)
			{
				Debug.Log("Patcher Postfix: Bootstrap");
			}
		}

		[HarmonyPatch(typeof(VoxelTycoon.Game.GameController), "StartShowWelcomeMessage")]
		class StartShowWelcomeMessage
		{
			static void Prefix(VoxelTycoon.Game.GameController __instance)
			{
				Debug.Log("Patcher Prefix: StartShowWelcomeMessage");
			}

			static void Postfix(VoxelTycoon.Game.GameController __instance)
			{
				Debug.Log("Patcher Postfix: StartShowWelcomeMessage");

				VoxelMultiplayer.Client.currentClient.newGame = true;
			}
		}
	}
}
