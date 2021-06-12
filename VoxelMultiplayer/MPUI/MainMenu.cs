using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.MainMenu.UI;
using VoxelTycoon.Serialization;
using VoxelTycoon.UI;

namespace VoxelMultiplayer.MPUI
{
    class MainMenu : Frame
    {
        private readonly List<Texture> _images = new List<Texture>()
        {
             R.Textures.MainMenuBackgrounds.Bg1,
             R.Textures.MainMenuBackgrounds.Bg2,
             R.Textures.MainMenuBackgrounds.Bg3,
             R.Textures.MainMenuBackgrounds.Bg4,
             R.Textures.MainMenuBackgrounds.Bg5,
             R.Textures.MainMenuBackgrounds.Bg6,
             R.Textures.MainMenuBackgrounds.Bg7,
             R.Textures.MainMenuBackgrounds.Bg8,
             R.Textures.MainMenuBackgrounds.Bg9,
             R.Textures.MainMenuBackgrounds.Bg10,
             R.Textures.MainMenuBackgrounds.Bg11,
             R.Textures.MainMenuBackgrounds.Bg12,
             R.Textures.MainMenuBackgrounds.Bg13,
             R.Textures.MainMenuBackgrounds.Bg14,
             R.Textures.MainMenuBackgrounds.Bg15,
             R.Textures.MainMenuBackgrounds.Bg16
        };
        private RawImage _background1;
        private RawImage _background2;
        private Transform _menuItemsContainer;
        private float _startTime;
        private VoxelTycoon.UI.Windows.Window _window;

        protected override void InitializeFrame()
        {
            base.InitializeFrame();
            _images.Shuffle((int)DateTime.Now.Ticks);
            BreakBatch = BatchBreakBehavior.BreakAfter;
            Transform transform = Instantiate(R.MainMenu.UI.MainMenuContent, this.transform).transform;
            transform.transform.Find<Text>("Menu/LogoWrapper/Logo/Subtitle").text = "Co-Op/Multiplayer";
            _background1 = transform.Find<RawImage>("Background1");
            _background2 = transform.Find<RawImage>("Background2");
            _menuItemsContainer = transform.Find("Menu/Items");
            AddMenuItems();
            InvalidateBackground();
        }

        protected void Start() => _startTime = Time.time;

        protected void Update()
        {
            InvalidateBackground();
        }

        private MainMenuItem AddMenuItem(string text, Action action) => AddMenuItem(text, null, action);

        private MainMenuItem AddMenuItem(string text, string secondaryText, Action action)
        {
            MainMenuItem mainMenuItem = Instantiate(R.MainMenu.UI.MainMenuItem, _menuItemsContainer);
            mainMenuItem.Initialize();
            mainMenuItem.Text = text;
            mainMenuItem.SecondaryText = secondaryText;
            mainMenuItem.Action = action;
            return mainMenuItem;
        }

        private void AddMenuItems()
        {
            _menuItemsContainer.Clear();
            AddMenuItem("Join Server", new Action(JoinGame));
            AddMenuItem("Host Server", new Action(HostGame));
            AddMenuItem("Game Settings", new Action(ShowSettings));
            AddMenuItem("Quit Game", new Action(Quit));
        }

        private void InvalidateBackground()
        {
            int num1 = 1;
            float b = 0.8f;
            float num2 = Mathf.Lerp(num1, b, 0.5f);
            int num3;
            float t = (num3 = (int)((Time.time - (double)_startTime) / 20.0)) % 1f;
            InvalidateBackground(_background1, _images[num3 % _images.Count], Mathf.Lerp(num2, b, t), 1f);
            InvalidateBackground(_background2, _images[(num3 + 1) % _images.Count], Mathf.Lerp(num1, num2, t), Mathf.Lerp(0.0f, 1f, Mathf.InverseLerp(0.96f, 1f, t)));
        }

        private void InvalidateBackground(RawImage background, Texture texture, float scale, float alpha)
        {
            if (!background.IsActive(alpha > 0.0))
                return;
            background.texture = texture;
            background.color = new Color(0.6f, 0.6f, 0.6f, alpha);
            float num1 = background.texture.width / (float)background.texture.height;
            float num2 = Screen.width / (float)Screen.height;
            int num3 = num1 < (double)num2 ? 1 : 0;
            Rect rect = new Rect(0.0f, 0.0f, 1f, 1f);
            if (num3 != 0)
            {
                rect.height = num1 / num2;
                rect.y = (float)((1.0 - rect.height) * 0.5);
            }
            else
            {
                rect.width = num2 / num1;
                rect.x = (float)((1.0 - rect.width) * 0.5);
            }
            background.uvRect = rect.Scale(scale);
        }

        private void HostGame()
        {
            GameSettingsWindow.ShowFor(WorldSettings.PreFilled(), true);

            LazyManager<Settings>.Current.DefautCompanyName.Value = "Voxel Multiplayer";
            LazyManager<Settings>.Current.DefautCompanyColor.Value = "FF0000";

            SaveManager.StartNewGame("GameScene", WorldSettings.PreFilled());
        }

        private void JoinGame()
        {
            SceneManager.LoadScene("GameScene");

            Debug.Log("Update(): Adding GameObject: Client");
            Client.client = new GameObject();
            Client.client.AddComponent<Network.Client>();
            DontDestroyOnLoad(Client.client);
        }

        private void Quit() => Helper.Quit();

        private void ShowSettings()
        {
            if (_window != null)
                _window.Close();
            _window = UIManager.Current.CreateFrame<SettingsWindow>(FrameAnchoring.Center);
            _window.Show();
            _window.Closed += () => _window = null;
        }
    }
}
