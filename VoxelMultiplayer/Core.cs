using UnityEngine;
using HarmonyLib;

/* Sync TODO (VoxelTycoon.Serialization.SaveMetadata)

	  this.Register<CompanyManager>(Manager<CompanyManager>.Current);
      this.Register<VisibilitySettings>(LazyManager<VisibilitySettings>.Current);
      this.Register<CoverManager>(LazyManager<CoverManager>.Current);
      this.Register<WorldManager>(Manager<WorldManager>.Current);
      this.Register<GameCameraView>(GameCameraView.Current);
      this.Register<BuildingManager>(LazyManager<BuildingManager>.Current);
      this.Register<NamesProvider>(LazyManager<NamesProvider>.Current);
      this.Register<DepositManager>(Manager<DepositManager>.Current);
      this.Register<ResearchManager>(LazyManager<ResearchManager>.Current);
      this.Register<CityManager>(Manager<CityManager>.Current);
      this.Register<RegionManager>(Manager<RegionManager>.Current);
      this.Register<VehicleStationLocationManager>(LazyManager<VehicleStationLocationManager>.Current);
      this.Register<CargoManager>(Manager<CargoManager>.Current);
      this.Register<TrackUnitManager>(LazyManager<TrackUnitManager>.Current);
      this.Register<NotificationManager>(Manager<NotificationManager>.Current);
      this.Register<NotificationSettings>(LazyManager<NotificationSettings>.Current);
      this.Register<VehicleRouteManager>(LazyManager<VehicleRouteManager>.Current);
      this.Register<StorageManager>(LazyManager<StorageManager>.Current);
      this.Register<StorageNetworkManager>(LazyManager<StorageNetworkManager>.Current);
      ? this.Register<TutorialManager>(LazyManager<TutorialManager>.Current);
*/

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
			Core.Multiplayer = new GameObject();
			Multiplayer.AddComponent<Client>();
			UnityEngine.Object.DontDestroyOnLoad(Multiplayer);

			Debug.Log("Load(): Adding GameObject: Logger");
			Core.Logger = new GameObject();
			Logger.AddComponent<Utils.Console>();
			UnityEngine.Object.DontDestroyOnLoad(Logger);

			Debug.Log("Load(): Initializing Patcher");
			Patcher = new Harmony("com.VoxelMultiplayer.VoxelMultiplayer.GamePatcher");
			Debug.Log("Load(): Patching Game Functions");
			Patcher.PatchAll();
		}

		public static void Unload()
		{
			Debug.Log("Unload(): Destroying GameObject: Multiplayer");
			GameObject.Destroy(Core.Multiplayer);
			Debug.Log("Unload(): Destroying GameObject: Logger");
			GameObject.Destroy(Core.Logger);

			Debug.Log("Unload(): Unpatching Game Functions");
			Patcher.UnpatchAll();
		}
	}
}
