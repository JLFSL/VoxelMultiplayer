using System.IO;
using System.Reflection;
using System.Threading;

using UnityEngine;

namespace VoxelMultiplayer
{
	public class Client : MonoBehaviour
	{
		public static Client currentClient;

		private static GameObject host;
		public static GameObject client;

		public static bool menuLoaded = false;
		private static bool serverStarted = false;
		public bool newGame { get; set; } = false;
		public static bool GameReady = false;

		private void Start()
		{
			Debug.Log("Client.Start(): Loaded");

			currentClient = this;
		}

		private void Update()
		{
			if (menuLoaded && !serverStarted && newGame)
			{
				Debug.Log("Update(): Adding GameObject: Host");
				host = new GameObject();
				host.AddComponent<Network.Server>();
				UnityEngine.Object.DontDestroyOnLoad(host);

				serverStarted = true;
				newGame = false;

				// Send map data to server
				Network.Server.ReceiveLatestMap(File.ReadAllBytes(VoxelTycoon.Serialization.SaveManager.SavesDirectory + "/" + VoxelTycoon.Serialization.SaveManager.Autosave().Filename));

				GameReady = true;

				// Quits the analytics manager so we don't get spammed
				Utility.Utils.InvokeMethod(VoxelTycoon.SceneControl.SceneController.Current, "OnApplicationQuit");
			}

			if (Input.GetKeyUp(KeyCode.F3)) // Load def. map
			{
				Debug.Log("Client.Update(): F3 pressed");

				FileInfo file = new FileInfo("C:\\Users\\Jimmy\\AppData\\LocalLow\\VoxelTycoon\\VoxelTycoon\\Saves\\xtfhf1d6x7r.sav");

				VoxelTycoon.Serialization.SaveMetadata save = VoxelTycoon.Serialization.SaveSerializer.ReadMetadata<VoxelTycoon.Serialization.SaveMetadata>(file.Name);
				save.Size = file.Length;

				Debug.Log("Client.Update(): Attempting to load savegame: " + file.FullName);
				Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(save));
				Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save)));
				VoxelTycoon.Game.UI.LoadGameHelper.TryLoad(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save));
			}

			if (Input.GetKeyUp(KeyCode.F5)) // Connect to server (and load game? :-))
			{
				Debug.Log("Client.Update(): F5 pressed");

				Debug.Log("Update(): Adding GameObject: Client");
				client = new GameObject();
				client.AddComponent<Network.Client>();
				UnityEngine.Object.DontDestroyOnLoad(client);
			}

			if (Input.GetKeyUp(KeyCode.F6))
			{
				
			}

			if (Input.GetKeyUp(KeyCode.F7))
			{
				
			}
		}
	}
}
