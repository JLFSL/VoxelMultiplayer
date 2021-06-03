using System;
using System.IO;

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


}
