﻿using System.IO;

using UnityEngine;

using VoxelTycoon.Game.UI;
using VoxelTycoon.Serialization;

namespace VoxelMultiplayer
{
    public class Client : MonoBehaviour
    {
        public static Client currentClient;

        public static GameObject host;
        public static GameObject client;

        public static bool menuLoaded = false;
        public static bool serverStarted = false;

        private void Start()
        {
            Debug.Log("Client.Start(): Loaded");
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F3)) // Load def. map
            {
                Debug.Log("Client.Update(): F3 pressed");

                FileInfo file = new FileInfo("C:\\Users\\Jimmy\\AppData\\LocalLow\\VoxelTycoon\\VoxelTycoon\\Saves\\xtfhf1d6x7r.sav");

                SaveMetadata save = SaveSerializer.ReadMetadata<SaveMetadata>(file.Name);
                save.Size = file.Length;

                Debug.Log("Client.Update(): Attempting to load savegame: " + file.FullName);
                Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(save));
                Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(SaveManager.GetFullMetadata(save)));
                LoadGameHelper.TryLoad(SaveManager.GetFullMetadata(save));
            }

            if (Input.GetKeyUp(KeyCode.F5)) // Connect to server (and load game? :-))
            {
                Debug.Log("Client.Update(): F5 pressed");

                Debug.Log("Update(): Adding GameObject: Client");
                client = new GameObject();
                client.AddComponent<Network.Client>();
                DontDestroyOnLoad(client);
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
