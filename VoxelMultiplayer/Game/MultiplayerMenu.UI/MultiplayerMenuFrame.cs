using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.MainMenu.UI;
using VoxelTycoon.UI;

namespace VoxelMultiplayer.Game.MultiplayerMenu.UI
{
    class MultiplayerMenuFrame : VoxelTycoon.UI.Frame
    {
        private readonly List<Texture> _images = new List<Texture>()
        {
            (Texture) R.Textures.MainMenuBackgrounds.Bg1,
            (Texture) R.Textures.MainMenuBackgrounds.Bg2,
            (Texture) R.Textures.MainMenuBackgrounds.Bg3,
            (Texture) R.Textures.MainMenuBackgrounds.Bg4,
            (Texture) R.Textures.MainMenuBackgrounds.Bg5,
            (Texture) R.Textures.MainMenuBackgrounds.Bg6,
            (Texture) R.Textures.MainMenuBackgrounds.Bg7,
            (Texture) R.Textures.MainMenuBackgrounds.Bg8,
            (Texture) R.Textures.MainMenuBackgrounds.Bg9,
            (Texture) R.Textures.MainMenuBackgrounds.Bg10,
            (Texture) R.Textures.MainMenuBackgrounds.Bg11,
            (Texture) R.Textures.MainMenuBackgrounds.Bg12,
            (Texture) R.Textures.MainMenuBackgrounds.Bg13,
            (Texture) R.Textures.MainMenuBackgrounds.Bg14,
            (Texture) R.Textures.MainMenuBackgrounds.Bg15,
            (Texture) R.Textures.MainMenuBackgrounds.Bg16
        };
        private RawImage _background1;
        private RawImage _background2;
        private Transform _menuItemsContainer;
        private float _startTime;
        private VoxelTycoon.UI.Windows.Window _window;

        protected override void InitializeFrame()
        {
            base.InitializeFrame();
            this._images.Shuffle<Texture>((int)DateTime.Now.Ticks);
            this.BreakBatch = BatchBreakBehavior.BreakAfter;
            Transform transform = UnityEngine.Object.Instantiate<CanvasRenderer>(R.MainMenu.UI.MainMenuContent, this.transform).transform;
            transform.transform.Find<UnityEngine.UI.Text>("Menu/LogoWrapper/Logo/Subtitle").text = "Co-Op/Multiplayer";
            this._background1 = transform.Find<RawImage>("Background1");
            this._background2 = transform.Find<RawImage>("Background2");
            this._menuItemsContainer = transform.Find("Menu/Items");
            this.AddMenuItems();
            this.InvalidateBackground();
        }

        protected void Start() => this._startTime = Time.time;

        protected void Update()
        {
            this.InvalidateBackground();
        }

        private MainMenuItem AddMenuItem(string text, Action action) => this.AddMenuItem(text, (string)null, action);

        private MainMenuItem AddMenuItem(string text, string secondaryText, Action action)
        {
            MainMenuItem mainMenuItem = UnityEngine.Object.Instantiate<MainMenuItem>(R.MainMenu.UI.MainMenuItem, this._menuItemsContainer);
            mainMenuItem.Initialize();
            mainMenuItem.Text = text;
            mainMenuItem.SecondaryText = secondaryText;
            mainMenuItem.Action = action;
            return mainMenuItem;
        }

        private void AddMenuItems()
        {
            this._menuItemsContainer.Clear();
            this.AddMenuItem("Join Server", new Action(this.JoinGame));
            this.AddMenuItem("Host Server", new Action(this.HostGame));
            this.AddMenuItem("Game Settings", new Action(this.ShowSettings));
            this.AddMenuItem("Quit Game", new Action(this.Quit));
        }

        private void InvalidateBackground()
        {
            int num1 = 1;
            float b = 0.8f;
            float num2 = Mathf.Lerp((float)num1, b, 0.5f);
            int num3;
            float t = (float)(num3 = (int)(((double)Time.time - (double)this._startTime) / 20.0)) % 1f;
            this.InvalidateBackground(this._background1, this._images[num3 % this._images.Count], Mathf.Lerp(num2, b, t), 1f);
            this.InvalidateBackground(this._background2, this._images[(num3 + 1) % this._images.Count], Mathf.Lerp((float)num1, num2, t), Mathf.Lerp(0.0f, 1f, Mathf.InverseLerp(0.96f, 1f, t)));
        }

        private void InvalidateBackground(RawImage background, Texture texture, float scale, float alpha)
        {
            if (!background.IsActive((double)alpha > 0.0))
                return;
            background.texture = texture;
            background.color = new Color(0.6f, 0.6f, 0.6f, alpha);
            float num1 = (float)background.texture.width / (float)background.texture.height;
            float num2 = (float)Screen.width / (float)Screen.height;
            int num3 = (double)num1 < (double)num2 ? 1 : 0;
            Rect rect = new Rect(0.0f, 0.0f, 1f, 1f);
            if (num3 != 0)
            {
                rect.height = num1 / num2;
                rect.y = (float)((1.0 - (double)rect.height) * 0.5);
            }
            else
            {
                rect.width = num2 / num1;
                rect.x = (float)((1.0 - (double)rect.width) * 0.5);
            }
            background.uvRect = rect.Scale(scale);
        }

        private void HostGame()
        {
            VoxelTycoon.Game.UI.GameSettingsWindow.ShowFor(VoxelTycoon.WorldSettings.PreFilled(), true);

            VoxelTycoon.LazyManager<VoxelTycoon.Settings>.Current.DefautCompanyName.Value = "Voxel Multiplayer";
            VoxelTycoon.LazyManager<VoxelTycoon.Settings>.Current.DefautCompanyColor.Value = "FF0000";

            VoxelTycoon.Serialization.SaveManager.StartNewGame("GameScene", VoxelTycoon.WorldSettings.PreFilled());
        }

        private void JoinGame()
        {
            Debug.LogWarning("Client.Update(): Connecting to Local Server");
            Thread client = new Thread(new ThreadStart(new Network.Client().Start));
            client.Start();
        }

        private void Quit() => Helper.Quit();

        private void ShowSettings()
        {
            if ((UnityEngine.Object)this._window != (UnityEngine.Object)null)
                this._window.Close();
            this._window = (VoxelTycoon.UI.Windows.Window)UIManager.Current.CreateFrame<SettingsWindow>(FrameAnchoring.Center);
            this._window.Show();
            this._window.Closed += (Action)(() => this._window = (VoxelTycoon.UI.Windows.Window)null);
        }
    }
}
