using System;
using System.IO;

using LiteNetLib;
using UnityEngine;

using VoxelMultiplayer.Injections;

using VoxelTycoon;
using VoxelTycoon.Buildings;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Serialization;
using VoxelTycoon.Tools;

namespace VoxelMultiplayer.Network.Packets
{
    class Helper
    {
        public static void SendPacketToOthers(object data, DeliveryMethod options)
        {
            if (_GameController.Playable)
            {
                if (Client.ClientPeer)
                {
                    Debug.LogError("Packet " + data.GetType() + " from Client");

                    ClientPeer.Manager.SendToAll(ClientPeer.Processor.Write(data), options, ClientPeer.Manager.GetEnumerator().Current);
                }
                else if (Client.ServerPeer)
                {
                    Debug.LogError("Packet " + data.GetType() + " from Server");

                    ServerPeer.Manager.SendToAll(ServerPeer.Processor.Write(data), options, ServerPeer.Manager.GetEnumerator().Current);
                }
            }
        }
    }

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
        public static Building[] Buildings = new Building[100000];

        //public int GameId { get; set; }
        public int ParentId { get; set; } = -1;

        public int AssetId { get; set; }
        public int Rotation { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int PositionZ { get; set; }

        public string DisplayName { get; set; }

        public void Build()
        {
            int Id = Buildings.Length - Buildings.Length + 1;

            Buildings[Id] = UnityEngine.Object.Instantiate(BuildingManager.Current.GetRotatedAsset<Building>(AssetId, (BuildingRotation)Rotation));

            //Utility.Utils.Invoke(prevBuilding, "Restore", new VoxelTycoon.Xyz(building.Position.X + 15, building.Position.Y + 15, building.Position.Z + 10), building.Id+1);
            //_temp.Build(new VoxelTycoon.Xyz(PositionX, PositionY, PositionZ));

            Buildings[Id].Company = Company.Current;
            Buildings[Id].City = VoxelTycoon.Manager<VoxelTycoon.RegionManager>.Current.GetClosestCity(new VoxelTycoon.Xz(PositionX, PositionZ));
            if(ParentId != -1)
                Buildings[Id].Parent = Buildings[ParentId];

            Utility.Utils.SetField(Buildings[Id], "DisplayName", DisplayName);

            Buildings[Id].Build(new Xyz(PositionX, PositionY, PositionZ));

            //GameId = LazyManager<BuildingManager>.Current.GenerateId();

            //Utility.Utils.Invoke(Buildings[Id], "Restore", new Xyz(PositionX, PositionY, PositionZ), GameId);
        }
    }
}
