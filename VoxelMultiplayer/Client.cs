using System.IO;
using System.Reflection;
using System.Threading;

using UnityEngine;

namespace VoxelMultiplayer
{
	public class Client : MonoBehaviour
	{
		public static Client currentClient;
		private static Network.Server host;

		private bool firstLoad = false;
		private bool serverStarted = false;
		public bool newGame { get; set; } = false;

		private void Start()
		{
			Debug.Log("Client.Start(): Loaded");

			currentClient = this;
		}

		private void Update()
		{
			if (!firstLoad && VoxelTycoon.SceneControl.SceneController.Current.Loaded)
			{
				Debug.Log("Client: Showing multiplayer menu");

				VoxelTycoon.UI.UIManager.Current.CreateFrame<VoxelMultiplayer.Game.MultiplayerMenu.UI.MultiplayerMenuFrame>(VoxelTycoon.UI.FrameAnchoring.Fullscreen).Show();

				firstLoad = true;
			}

			if (firstLoad && !serverStarted && newGame)
			{
				Debug.LogWarning("Client.Update(): Starting Local Server");
				host = new Network.Server();
				Thread server = new Thread(new ThreadStart(host.Start));
				server.Start();

				serverStarted = true;
				newGame = false;

				// Send map data to server
				byte[] bytes = File.ReadAllBytes(VoxelTycoon.Serialization.SaveManager.SavesDirectory + "/" + VoxelTycoon.Serialization.SaveManager.Autosave().Filename);
				host.ReceiveLatestMap(bytes);

				//VoxelTycoon.Manager<VoxelTycoon.AnalyticsManager>
			}

			/*if (Input.GetKeyUp(KeyCode.F3)) // Load def. map
			{
				Debug.Log("Client.Update(): F3 pressed");

				FileInfo file = new FileInfo("C:\\Users\\Jimmy\\AppData\\LocalLow\\VoxelTycoon\\VoxelTycoon\\Saves\\xtfhf1d6x7r.sav");

				VoxelTycoon.Serialization.SaveMetadata save = VoxelTycoon.Serialization.SaveSerializer.ReadMetadata<VoxelTycoon.Serialization.SaveMetadata>(file.Name);
				save.Size = file.Length;

				Debug.Log("Client.Update(): Attempting to load savegame: " + file.FullName);
				Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(save));
				Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save)));
				VoxelTycoon.Game.UI.LoadGameHelper.TryLoad(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save));
			}*/

			if (Input.GetKeyUp(KeyCode.F5)) // Connect to server (and load game? :-))
			{
				Debug.Log("Client.Update(): F5 pressed");

				Debug.LogWarning("Client.Update(): Connecting to Local Server");
				Thread client = new Thread(new ThreadStart(new Network.Client().Start));
				client.Start();
			}

			if (Input.GetKeyUp(KeyCode.F6))
			{
				/* Unfortunately only sends metadata instead of a whole save. Took me way too long to figure this out.
				 * 
				 * VoxelTycoon.Serialization.ExtendedSaveMetadata data = Game.Serialization.SaveManager.CreateMetadataForCurrentState("tempsave");
				data.Screenshot = null;
				string serialized =  Newtonsoft.Json.JsonConvert.SerializeObject(data);
				Debug.LogError(serialized);
				host.ReceiveLatestMap(serialized);*/

				byte[] bytes = File.ReadAllBytes(VoxelTycoon.Serialization.SaveManager.SavesDirectory + "/" + VoxelTycoon.Serialization.SaveManager.Autosave().Filename);
				host.ReceiveLatestMap(bytes);
			}

			if (Input.GetKeyUp(KeyCode.F7))
			{
				Debug.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(VoxelTycoon.Serialization.SaveSerializer.Current));
			}
		}
	}
}
