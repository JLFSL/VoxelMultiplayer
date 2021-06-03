using HarmonyLib;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace VoxelMultiplayer.Game.Game
{
    class GameController : VoxelTycoon.SceneControl.SceneController
    {
        private static VoxelTycoon.Game.GameController _gameController;

        private float _lastSaveTime;

        public override IEnumerator Bootstrap()
        {
            VoxelTycoon.Game.GameController gameController = _gameController;

            VoxelTycoon.Integrations.Steam.SteamManager.CreateSafe();
            VoxelTycoon.Integrations.Discord.DiscordManager.InitializeSafe();

            VoxelTycoon.Manager<VoxelTycoon.Audio.SoundManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.CountersManager>.Initialize();

            this.SetupUI();

            VoxelTycoon.Manager<VoxelTycoon.LightingManager>.Initialize();

            GC.Collect();

            AsyncOperation op = UnityEngine.Resources.UnloadUnusedAssets();
            while (!op.isDone)
                yield return (object)null;

            VoxelTycoon.DebugSettings.Load();
            VoxelTycoon.DebugSettings.IgnoreRegions = false;

            VoxelTycoon.Manager<VoxelTycoon.AssetLibrary>.Initialize();
            IEnumerable<VoxelTycoon.AssetManagement.Pack> enabledPacks = VoxelTycoon.AssetManagement.EnabledPacksPerSaveHelper.GetEnabledPacks();
            VoxelTycoon.AssetLibraryHelper.RegisterHandlers();

            /*try
            {
                new VoxelTycoon.Modding.ModLoader().Load(enabledPacks);
            }
            catch (Exception ex)
            {
                this.ProgressReporter.ReportException(ex);
            }*/

            yield return Utility.Utils.InvokeMethod(VoxelTycoon.Manager<VoxelTycoon.AssetLibrary>.Current, "Load", this.ProgressReporter, enabledPacks);

            try
            {
                VoxelTycoon.LazyManager<VoxelTycoon.Tracks.VehicleUnitManager>.Current.OnAssetsLoaded();
                VoxelTycoon.LazyManager<VoxelTycoon.Tracks.VehicleRecipeManager>.Current.OnAssetsLoaded();
                VoxelTycoon.LazyManager<VoxelTycoon.Researches.ResearchManager>.Current.OnAssetsLoaded();
                VoxelTycoon.LazyManager<VoxelTycoon.Cities.CityStoreSpawnInfoManager>.Current.OnAssetsLoaded();
            }
            catch (Exception ex)
            {
               this.ProgressReporter.ReportException(new Exception("Can't finalize asset loading", ex));
            }

            Utility.Utils.InvokeMethod(VoxelTycoon.Game.GameUI.Current, "OnLocalizationReady", null);
            VoxelTycoon.Manager<VoxelTycoon.Game.UI.UIFormat>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.BiomeManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.RecipeManager>.Initialize();

            if (VoxelTycoon.Serialization.SaveManager.LoadingMetadata != null)
            {
                this.ProgressReporter.ReportOperationStarted((string)VoxelTycoon.S.PreloaderLoadingSavedGame);
                yield return (object)null;
            }

            VoxelTycoon.Manager<VoxelTycoon.WindManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.WorldManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Tracks.Conveyors.CargoManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Notifications.NotificationManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Game.UI.Notifications.NotificationPopupManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Cities.CityManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Deposits.DepositManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.CompanyManager>.Initialize();
            VoxelTycoon.GameCameraView.Initialize();

            bool newGame = true;
            Utility.Utils.InvokeMethod(VoxelTycoon.LazyManager<VoxelTycoon.Modding.ModManager>.Current, "OnGameStarting");
            try
            {
                //if (VoxelTycoon.Serialization.SaveManager.LoadingMetadata == null)
                //   newGame = false;

                // can't get this to work? - VoxelTycoon.Serialization.SaveManager.LoadInternal
                newGame = true;
            }
            catch (Exception ex)
            {
                this.ProgressReporter.ReportException(new Exception((string)VoxelTycoon.S.PreloaderCantLoadSavedGame, ex));
            }

            VoxelTycoon.CameraController.Current.View = (VoxelTycoon.ICameraView)VoxelTycoon.GameCameraView.Current;
            VoxelTycoon.CameraController.Current.DefaultView = (VoxelTycoon.ICameraView)VoxelTycoon.GameCameraView.Current;
            VoxelTycoon.CameraController.Current.SwitchToDynamicListener();
            VoxelTycoon.Manager<VoxelTycoon.WorldManager>.Current.OnWorldSettingsInitialized();
            VoxelTycoon.LazyManager<VoxelTycoon.NamesProvider>.Current.OnWorldSettingsInitialized();
            VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.OnWorldSettingsInitialized();
            VoxelTycoon.LazyManager<VoxelTycoon.PlantManager>.Current.OnWorldSettingsInitialized();

            if (newGame)
            {
                VoxelTycoon.Company company = new VoxelTycoon.Company()
                {
                    Id = VoxelTycoon.Manager<VoxelTycoon.CompanyManager>.Current.GenerateId(),
                    Name = (string)VoxelTycoon.LazyManager<VoxelTycoon.Settings>.Current.DefautCompanyName,
                    Color = (Color)VoxelTycoon.ColorHelper.FromHexString((string)VoxelTycoon.LazyManager<VoxelTycoon.Settings>.Current.DefautCompanyColor)
                };

                int loanCount = Mathf.RoundToInt(30f * VoxelTycoon.WorldSettings.Current.StartupCapitalMultiplier);
                int extraLoanCount = Mathf.RoundToInt(10f * VoxelTycoon.WorldSettings.Current.LoanMultiplier);
                double loanInterest = 0.01 * (double)VoxelTycoon.WorldSettings.Current.LoanInterestMultiplier;

                company.InitializeLoan(loanCount, extraLoanCount, 25000.0, loanInterest);

                VoxelTycoon.Manager<VoxelTycoon.CompanyManager>.Current.Register(company);
                VoxelTycoon.Manager<VoxelTycoon.CompanyManager>.Current.SetCurrentCompany(company);

                if (VoxelTycoon.WorldSettings.Current.ResearchDifficulty == VoxelTycoon.Researches.ResearchDifficultyMode.AllCompleted)
                    VoxelTycoon.LazyManager<VoxelTycoon.Researches.ResearchManager>.Current.CompleteAll();

                if (VoxelTycoon.WorldSettings.Current.SignalDifficulty == VoxelTycoon.SignalDifficultyMode.All)
                {
                    VoxelTycoon.Researches.Research research = VoxelTycoon.Manager<VoxelTycoon.AssetLibrary>.Current.Get<VoxelTycoon.Researches.Research>("base/rail_signals_2.research");
                    if (research != null && !VoxelTycoon.LazyManager<VoxelTycoon.Researches.ResearchManager>.Current.IsCompleted(research))
                        VoxelTycoon.LazyManager<VoxelTycoon.Researches.ResearchManager>.Current.Complete(research);
                }
                VoxelTycoon.LazyManager<VoxelTycoon.Researches.ResearchManager>.Current.ResearchCompletedFlag = false;

                this.ProgressReporter.ReportOperationStarted((string)VoxelTycoon.S.Exploring);
                yield return (object)null;

                VoxelTycoon.Region regionByIndex = VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.CreateRegionByIndex(new VoxelTycoon.Xz(0, 0), new VoxelTycoon.Xz(1, 1));
                VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.UnlockRegion(regionByIndex);
                VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.GenerateChunks();
                VoxelTycoon.Cities.City city = regionByIndex.Cities.ToList().OrderByDescending<VoxelTycoon.Cities.City, int>((Func<VoxelTycoon.Cities.City, int>)(x => x.Population)).FirstOrDefault<VoxelTycoon.Cities.City>();
                VoxelTycoon.GameCameraView.Current.SetTarget((Vector3)(VoxelTycoon.Xyz)(city != null ? city.Position : regionByIndex.Center), false);
            } 
            yield return (object)null;

            VoxelTycoon.LazyManager<VoxelTycoon.SoundtrackPlayer>.Current.Play(VoxelTycoon.Manager<VoxelTycoon.AssetLibrary>.Current.Get<VoxelTycoon.Audio.Playlist>("base/ost_vol1.playlist"));

            gameController.SpawnImmediately<VoxelTycoon.AmbientPlayer>(VoxelTycoon.R.Prefabs.AmbientPlayer);

            VoxelTycoon.LazyManager<VoxelTycoon.TimeManager>.Current.Start();
            VoxelTycoon.Manager<VoxelTycoon.CompanyManager>.Current.Start();

            this._lastSaveTime = Time.unscaledTime;
            this.StartPlaytimeTracking(enabledPacks);

            VoxelTycoon.UI.UIManager.Current.Interactable = true;

            if (newGame)
                gameController.StartCoroutine(this.StartShowWelcomeMessage());
            else if (VoxelTycoon.LazyManager<VoxelTycoon.Game.TutorialManager>.Current.Tutorial != null)
                VoxelTycoon.Game.UI.TutorialWindow.ShowUnique();

            Debug.Log((object)string.Format("Seed: {0} ({1})", (object)VoxelTycoon.WorldSettings.Current.SeedString, (object)VoxelTycoon.WorldSettings.Current.Seed));

            // Since the game won't do it for us automatically for some reason
            this.OnLoaded();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.ProgressReporter.OnSceneLoaded();
            VoxelTycoon.Game.GameUI.Current.Show();
        }

        private IEnumerator StartShowWelcomeMessage()
        {
            yield return (object)new WaitForSecondsRealtime(2f);
            VoxelTycoon.Audio.SoundManager current = VoxelTycoon.Manager<VoxelTycoon.Audio.SoundManager>.Current;
            VoxelTycoon.Audio.Sound sound = new VoxelTycoon.Audio.Sound();
            sound.Clip = VoxelTycoon.R.Audio.Raw.Notification;
            Vector3? position = new Vector3?();
            current.PlayOnce(sound, position);
            VoxelTycoon.Game.UI.RegionDetailsWindow.ShowWelcome(VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.HomeRegion);

            VoxelMultiplayer.Client.currentClient.newGame = true;
        }

        private void SetupUI()
        {
            VoxelTycoon.UI.UIManager.Initialize();
            VoxelTycoon.UI.UIManager.Current.Interactable = false;
            VoxelTycoon.Manager<VoxelTycoon.Game.UI.StorageNetworking.StorageNetworkVisualizationManager>.Initialize();
            this.SetupIndicatorManager();
            VoxelTycoon.Game.UI.GameSceneLoadingIndicator frame = VoxelTycoon.UI.UIManager.Current.CreateFrame<VoxelTycoon.Game.UI.GameSceneLoadingIndicator>(VoxelTycoon.UI.FrameAnchoring.Center);
            frame.Priority = 200;
            this.ProgressReporter = (VoxelTycoon.SceneControl.ISceneLoadingProgressReporter)frame;
            frame.Show();
            VoxelTycoon.UI.UIManager.Current.CreateFrame<VoxelTycoon.Game.GameUI>(VoxelTycoon.UI.FrameAnchoring.Center).Priority = 100;
            VoxelTycoon.Utils.GraphyFrame.Spawn();
        }

        private void SetupIndicatorManager()
        {
            VoxelTycoon.Manager<VoxelTycoon.UI.IndicatorManager>.Initialize();
            VoxelTycoon.Manager<VoxelTycoon.Game.UI.IndicatorHelper>.Initialize();
        }

        private void StartPlaytimeTracking(IEnumerable<VoxelTycoon.AssetManagement.Pack> enabledPacks) => VoxelTycoon.AssetManagement.SteamWorkshopHelper.StartPlaytimeTracking(enabledPacks);

        [HarmonyPatch(typeof(VoxelTycoon.Game.GameController), "Bootstrap")]
        class Patch
        {
            static bool Prefix(VoxelTycoon.Game.GameController __instance, ref IEnumerator __result)
            {
                Debug.Log("Patcher Prefix: Bootstrap");

                GameController._gameController = __instance;
                __result = new GameController().Bootstrap();
                return false;
            }
        }

        [HarmonyPatch(typeof(VoxelTycoon.Game.GameController), "StartShowWelcomeMessage")]
        class Patch2
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
