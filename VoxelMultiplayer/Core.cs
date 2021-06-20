using UnityEngine;
using HarmonyLib;

namespace VoxelMultiplayer
{
    public class Core
    {
        public static GameObject Multiplayer;
        public static GameObject Logger;

        private static Harmony Patcher;

        public static void Load()
        {
            Debug.Log("Load(): Adding GameObject: Multiplayer");
            Multiplayer = new GameObject();
            Multiplayer.AddComponent<Client>();
            Object.DontDestroyOnLoad(Multiplayer);

            Debug.Log("Load(): Adding GameObject: Logger");
            Logger = new GameObject();
            Logger.AddComponent<Utility.Console>();
            Object.DontDestroyOnLoad(Logger);

            Debug.Log("Load(): Initializing Patcher");
            Patcher = new Harmony("com.VoxelMultiplayer.VoxelMultiplayer.GamePatcher");
            Debug.Log("Load(): Patching Game Functions");
            Patcher.PatchAll();
        }

        public static void Unload()
        {
            Debug.Log("Unload(): Destroying GameObject: Multiplayer");
            Object.Destroy(Multiplayer);
            Debug.Log("Unload(): Destroying GameObject: Logger");
            Object.Destroy(Logger);

            Debug.Log("Unload(): Unpatching Game Functions");
            Patcher.UnpatchAll();
        }
    }
}
