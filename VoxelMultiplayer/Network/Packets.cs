using System;
using System.IO;

using LiteNetLib.Utils;

using UnityEngine;

namespace VoxelMultiplayer.Network.Packets
{
    class MapData
    {
        public int Length { get; set; }
        public byte[] Data { get; set; }

        public void LoadMap()
        {
            Debug.Log(this.Length);

            string tempname = "mp" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString() + ".sav";

            if (Data != null)
            {
                string dirFile = VoxelTycoon.Serialization.SaveManager.SavesDirectory + @"\" + tempname;
                Debug.Log(VoxelTycoon.Serialization.SaveManager.SavesDirectory + " " + dirFile);

                if (Utility.Utils.ByteArrayToFile(dirFile, Data))
                {
                    FileInfo file = new FileInfo(dirFile);
                    Debug.Log(file.FullName);

                    VoxelTycoon.Serialization.SaveMetadata save = VoxelTycoon.Serialization.SaveSerializer.ReadMetadata<VoxelTycoon.Serialization.SaveMetadata>(file.FullName);

                    VoxelTycoon.Game.UI.LoadGameHelper.TryLoad(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save));
                }
            }
        }
    }

    class BuildingData
    {
        public int AssetId { get; set; }
        public int Rotation { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int PositionZ { get; set; }

        public void Build()
        {
            VoxelTycoon.Buildings.Building _temp = UnityEngine.Object.Instantiate<VoxelTycoon.Buildings.Building>(VoxelTycoon.Buildings.BuildingManager.Current.GetRotatedAsset<VoxelTycoon.Buildings.Building>(AssetId, (VoxelTycoon.BuildingRotation)Rotation));
            //Utility.Utils.InvokeMethod(prevBuilding, "Restore", new VoxelTycoon.Xyz(building.Position.X + 15, building.Position.Y + 15, building.Position.Z + 10), building.Id+1);
            _temp.Build(new VoxelTycoon.Xyz(PositionX, PositionY, PositionZ));
        }
    }
}
