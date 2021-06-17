using System;
using System.IO;

using UnityEngine;

using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Serialization;
using VoxelTycoon.Tools;

namespace VoxelMultiplayer.Network.Packets
{
    class MapData
    {
        public static FileInfo TemporarySave;

        public int Length { get; set; }
        public byte[] Data { get; set; }

        public void LoadMap()
        {
            Debug.Log(Length);

            string tempname = "mp" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString() + ".sav";

            if (Data != null)
            {
                string dirFile = SaveManager.SavesDirectory + @"\" + tempname;

                if (Utility.Utils.ByteArrayToFile(dirFile, Data))
                {
                    TemporarySave = new FileInfo(dirFile);
                    SaveMetadata save = SaveSerializer.ReadMetadata<SaveMetadata>(TemporarySave.FullName);
                    LoadGameHelper.TryLoad(SaveManager.GetFullMetadata(save));
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
            Building _temp = UnityEngine.Object.Instantiate(BuildingManager.Current.GetRotatedAsset<Building>(AssetId, (BuildingRotation)Rotation));
            //Utility.Utils.Invoke(prevBuilding, "Restore", new VoxelTycoon.Xyz(building.Position.X + 15, building.Position.Y + 15, building.Position.Z + 10), building.Id+1);
            //_temp.Build(new VoxelTycoon.Xyz(PositionX, PositionY, PositionZ));

            _temp.Company = Company.Current;
            _temp.City = VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.GetClosestCity(new VoxelTycoon.Xz(PositionX, PositionZ));

            _temp.Build(new Xyz(PositionX, PositionY, PositionZ));
            
            //Utility.Utils.Invoke(_temp, "Restore", new Xyz(PositionX, PositionY, PositionZ), LazyManager<BuildingManager>.Current.GenerateId());
        }
    }

    class ToolExecuteData
    {
        public bool Predicate { get; set; }
        public double Price { get; set; }
        public int BudgetItem { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public void Execute()
        {
            if (Predicate && Company.Current.HasEnoughMoney(Price))
            {
                Company.Current.AddMoney(0.0 - Price, (BudgetItem)BudgetItem);
                LazyManager<ToolHintManager>.Current.Hide();
                if (Price != 0.0)
                {
                    LazyManager<ToolHintManager>.Current.Float(new Vector3(PositionX, PositionY, PositionZ), Price);
                }
                /*if (successSound != null)
                {
                    Manager<SoundManager>.Current.PlayOnce(successSound);
                }*/
            }
            /*if (failureSound != null)
            {
                Manager<SoundManager>.Current.PlayOnce(failureSound);
            }*/
        }
    }

    class BuilderToolData
    {
        //BuildingRecipe
    }
}
