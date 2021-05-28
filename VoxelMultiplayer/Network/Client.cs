using System;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

using UnityEngine;

using LiteNetLib;
using System.Collections.Generic;

namespace VoxelMultiplayer.Network
{
    public class Client
    {
        private static EventBasedNetListener Listener;
        private static NetManager Manager;

        private readonly string Host = "localhost";
        private readonly int Port = 23020;
        private readonly string Key = "";

        //private readonly int maxStringLength = 65535;

        public bool closeConnection = false;

        public static byte[] currentMapData;

        public void Start()
        {
            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener);

            Manager.Start();
            Manager.Connect(Host, Port, Key);
            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Debug.LogError(dataReader.AvailableBytes);
                int length = dataReader.GetInt();
                Debug.LogError(dataReader.AvailableBytes + " " + length);
                //dataReader.GetBytes(currentMapData, length);

                currentMapData = dataReader.GetRemainingBytes();

                Debug.LogError(currentMapData.Length);

                string tempname = "mp" + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString() + ".sav";

                if (currentMapData != null)
                {
                    string dirFile = VoxelTycoon.Serialization.SaveManager.SavesDirectory + @"\" + tempname;
                    Debug.LogError(VoxelTycoon.Serialization.SaveManager.SavesDirectory + " " + dirFile);

                    if (ByteArrayToFile(dirFile, currentMapData))
                    {
                        FileInfo file = new FileInfo(dirFile);
                        Debug.LogError(file.FullName);

                        VoxelTycoon.Serialization.SaveMetadata save = VoxelTycoon.Serialization.SaveSerializer.ReadMetadata<VoxelTycoon.Serialization.SaveMetadata>(file.FullName);

                        VoxelTycoon.Game.UI.LoadGameHelper.TryLoad(VoxelTycoon.Serialization.SaveManager.GetFullMetadata(save));
                    }
                }

                dataReader.Recycle();
            };

            while (true)
            {
                Manager.PollEvents();
                Thread.Sleep(15);

                if (closeConnection)
                    break;
            }
        }

        public void Stop()
        {
            closeConnection = true;
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in process: " + ex);
                return false;
            }
        }
    }
}