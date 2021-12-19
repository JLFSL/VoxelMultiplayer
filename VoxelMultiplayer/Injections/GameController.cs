using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using HarmonyLib;
using UnityEngine;

using VoxelTycoon;
using VoxelTycoon.AssetManagement;
using VoxelTycoon.Audio;
using VoxelTycoon.Cities;
using VoxelTycoon.Deposits;
using VoxelTycoon.Game;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Game.UI.Notifications;
using VoxelTycoon.Game.UI.StorageNetworking;
using VoxelTycoon.Integrations.Discord;
using VoxelTycoon.Integrations.Steam;
using VoxelTycoon.Modding;
using VoxelTycoon.Notifications;
using VoxelTycoon.Researches;
using VoxelTycoon.SceneControl;
using VoxelTycoon.Serialization;
using VoxelTycoon.Tracks;
using VoxelTycoon.Tracks.Conveyors;
using VoxelTycoon.UI;
using VoxelTycoon.UI.Windows;
using VoxelTycoon.Utils;

namespace VoxelMultiplayer.Injections
{
    [HarmonyPatch]
    class _GameController : GameController
    {
        public static _GameController Instance { get; private set; }
        public static bool Playable = false;

        public bool NewGame = true;

        public override IEnumerator Bootstrap()
        {
            Utility.Utils.SetField(typeof(SceneController), "Current", this);
            Instance = this;

            SteamManager.CreateSafe();
            DiscordManager.InitializeSafe();
            Manager<SoundManager>.Initialize();
            Manager<CountersManager>.Initialize();
            Manager<LightingManager>.Initialize();

            SetupUI();
            GC.Collect();

            AsyncOperation op = Resources.UnloadUnusedAssets();
            while (!op.isDone)
            {
                yield return null;
            }

            DebugSettings.Load();
            DebugSettings.IgnoreRegions = false;

            Manager<AssetLibrary>.Initialize();
            IEnumerable<Pack> enabledPacks = EnabledPacksPerSaveHelper.GetEnabledPacks();
            AssetLibraryHelper.RegisterHandlers();

            /*try
			{
				new ModLoader().Load(enabledPacks);
			}
			catch (Exception exception)
			{
				base.ProgressReporter.ReportException(exception);
			}*/

            yield return Utility.Utils.Invoke(Manager<AssetLibrary>.Current, "Load", ProgressReporter, enabledPacks);
            try
            {
                LazyManager<VehicleUnitManager>.Current.OnAssetsLoaded();
                LazyManager<VehicleRecipeManager>.Current.OnAssetsLoaded();
                LazyManager<ResearchManager>.Current.OnAssetsLoaded();
                LazyManager<CityStoreSpawnInfoManager>.Current.OnAssetsLoaded();
            }
            catch (Exception innerException)
            {
                ProgressReporter.ReportException(new Exception("Can't finalize asset loading", innerException));
            }

            Utility.Utils.Invoke(GameUI.Current, "OnLocalizationReady", null);
            if (SaveManager.LoadingMetadata != null)
            {
                ProgressReporter.ReportOperationStarted(S.PreloaderLoadingSavedGame);
                yield return null;
            }

            Manager<UIFormat>.Initialize();
            Manager<BiomeManager>.Initialize();
            Manager<RecipeManager>.Initialize();
            Manager<WindManager>.Initialize();
            Manager<WorldManager>.Initialize();
            Manager<CargoManager>.Initialize();
            Manager<VehicleMaintenanceManager>.Initialize();
            Manager<NotificationManager>.Initialize();
            Manager<NotificationPopupManager>.Initialize();
            Manager<CityManager>.Initialize();
            Manager<DepositManager>.Initialize();
            Manager<RegionManager>.Initialize();
            Manager<CompanyManager>.Initialize();
            GameCameraView.Initialize();

            //Utility.Utils.Invoke(LazyManager<ModManager>.Current, "OnGameStarting");
            try
            {
                if (SaveManager.LoadingMetadata != null)
                {
                    NewGame = false;
                    SaveSerializer.Read(SaveManager.SavesDirectory + "/" + SaveManager.LoadingMetadata.Filename, SaveManager.GetFullMetadata(SaveManager.LoadingMetadata));
                }
            }
            catch (Exception innerException2)
            {
                ProgressReporter.ReportException(new Exception(S.PreloaderCantLoadSavedGame, innerException2));
            }

            CameraController.Current.View = GameCameraView.Current;
            CameraController.Current.DefaultView = GameCameraView.Current;
            CameraController.Current.SwitchToDynamicListener();

            Manager<WorldManager>.Current.OnWorldSettingsInitialized();
            LazyManager<NamesProvider>.Current.OnWorldSettingsInitialized();
            Manager<RegionManager>.Current.OnWorldSettingsInitialized();
            LazyManager<PlantManager>.Current.OnWorldSettingsInitialized();

            if (NewGame)
            {
                Company company = new Company
                {
                    Id = Manager<CompanyManager>.Current.GenerateId(),
                    Name = LazyManager<Settings>.Current.DefautCompanyName,
                    Color = ColorHelper.FromHexString(LazyManager<Settings>.Current.DefautCompanyColor)
                };
                int loanCount = Mathf.RoundToInt(30f * WorldSettings.Current.StartupCapitalMultiplier);
                int extraLoanCount = Mathf.RoundToInt(10f * WorldSettings.Current.LoanMultiplier);
                double loanInterest = 0.01 * WorldSettings.Current.LoanInterestMultiplier;
                company.InitializeLoan(loanCount, extraLoanCount, 25000.0, loanInterest);
                Manager<CompanyManager>.Current.Register(company);
                Manager<CompanyManager>.Current.SetCurrentCompany(company);
                if (WorldSettings.Current.ResearchDifficulty == ResearchDifficultyMode.AllCompleted)
                {
                    LazyManager<ResearchManager>.Current.CompleteAll();
                }
                if (WorldSettings.Current.SignalDifficulty == SignalDifficultyMode.All)
                {
                    Research research = Manager<AssetLibrary>.Current.Get<Research>("base/rail_signals_2.research");
                    if (research != null && !LazyManager<ResearchManager>.Current.IsCompleted(research))
                    {
                        LazyManager<ResearchManager>.Current.Complete(research);
                    }
                }
                LazyManager<ResearchManager>.Current.ResearchCompletedFlag = false;
                ProgressReporter.ReportOperationStarted(S.Exploring);
                yield return null;
                Region region = Manager<RegionManager>.Current.CreateRegionByIndex(new Xz(0, 0), new Xz(1, 1));
                Manager<RegionManager>.Current.UnlockRegion(region);
                Manager<RegionManager>.Current.GenerateChunks();
                City city = (from x in region.Cities.ToList()
                             orderby x.Population descending
                             select x).FirstOrDefault();
                GameCameraView.Current.SetTarget((Xyz)(city?.Position ?? region.Center), animate: false);
            }
            yield return null;

            LazyManager<SoundtrackPlayer>.Current.Play(Manager<AssetLibrary>.Current.Get<Playlist>("base/ost_vol1.playlist"));
            SpawnImmediately(R.Prefabs.AmbientPlayer);

            LazyManager<TimeManager>.Current.Start();
            Manager<CompanyManager>.Current.Start();

            //_lastSaveTime = Time.unscaledTime;
            //StartPlaytimeTracking(enabledPacks);

            UIManager.Current.Interactable = true;

            if (NewGame)
            {
                yield return new WaitForSecondsRealtime(2f);
                Manager<SoundManager>.Current.PlayOnce(new Sound { Clip = R.Audio.Raw.Notification }, new Vector3?());
                RegionDetailsWindow.ShowWelcome(Manager<RegionManager>.Current.HomeRegion);

                if(Network.ServerPeer.Manager != null)
                    Network.ServerPeer.Manager.Stop();

                ProgressReporter.ReportOperationStarted("Starting Server");
                Debug.Log("Client.Update(): Adding GameObject: Host");
                Client.ServerPeer = new GameObject();
                Client.ServerPeer.AddComponent<Network.ServerPeer>();
                DontDestroyOnLoad(Client.ServerPeer);

                Client.serverStarted = true;

                // Send map data to server
                /*ExtendedSaveMetadata _save = SaveManager.Autosave();
                Debug.LogError(SaveManager.SavesDirectory + " " + _save.Filename);
                Network.ServerPeer.ReceiveLatestMap(File.ReadAllBytes(SaveManager.SavesDirectory + "/" + _save.Filename));
                File.Delete(SaveManager.SavesDirectory + "/" + _save.Filename);*/
            }
            else if (LazyManager<TutorialManager>.Current.Tutorial != null)
            {
                TutorialWindow.ShowUnique();

                if (Network.ClientPeer.Manager != null)
                    File.Delete(Network.Packets.MapData.TemporarySave.FullName);
            }

            Playable = true;
            Debug.Log($"Seed: {WorldSettings.Current.SeedString} ({WorldSettings.Current.Seed})");

            OnLoaded();
        }

        protected override void Update()
        {
            base.Update();
            if (LazyManager<InputManager>.Current.GetKeyDown(LazyManager<Settings>.Current.ToggleFreeCameraKey))
            {
                ToggleFreeCamera();
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            ProgressReporter.OnSceneLoaded();
            GameUI.Current.Show();
            //LazyManager<ModManager>.Current.OnGameStarted();
        }

        private void SetupUI()
        {
            UIManager.Initialize();
            UIManager.Current.Interactable = false;
            Manager<StorageNetworkVisualizationManager>.Initialize();
            SetupIndicatorManager();
            GameSceneLoadingIndicator gameSceneLoadingIndicator = UIManager.Current.CreateFrame<GameSceneLoadingIndicator>(FrameAnchoring.Center);
            gameSceneLoadingIndicator.Priority = 200;
            ProgressReporter = gameSceneLoadingIndicator;
            gameSceneLoadingIndicator.Show();
            UIManager.Current.CreateFrame<GameUI>(FrameAnchoring.Center).Priority = 100;
            GraphyFrame.Spawn();
        }

        private void SetupIndicatorManager()
        {
            Manager<IndicatorManager>.Initialize();
            Manager<IndicatorHelper>.Initialize();
        }

        [HarmonyPatch(typeof(GameController))]
        [HarmonyPatch("Bootstrap")]
        static bool Prefix(ref IEnumerator __result)
        {
            Debug.Log("_GameController: Bootstrap Patch");

            _GameController _newGC = new _GameController();
            __result = _newGC.Bootstrap();
            Instance = _newGC;
            return false;
        }
    }
}
