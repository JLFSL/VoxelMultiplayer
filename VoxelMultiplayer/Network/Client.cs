using System;
using System.IO;
using System.Threading;

using UnityEngine;

using LiteNetLib;
using LiteNetLib.Utils;

namespace VoxelMultiplayer.Network
{
    public class Client : MonoBehaviour
    {
        private static EventBasedNetListener Listener;
        private static NetManager Manager;
        private static NetPacketProcessor Processor;

        private readonly string Host = "localhost";
        private readonly int Port = 23020;
        private readonly string Key = "";

        //private readonly int maxStringLength = 65535;

        public bool closeConnection = false;

        public static byte[] currentMapData;
        public static FileInfo _file;

        private void Start()
        {
            Debug.LogWarning("Client.Update(): Connecting to Local Server");

            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener);
            Processor = new NetPacketProcessor();

            Manager.Start();
            Manager.Connect(Host, Port, Key);
            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Processor.ReadAllPackets(dataReader, fromPeer);

                dataReader.Recycle();
            };

            Processor.SubscribeReusable<Packets.MapData>((data) =>
            {
                data.LoadMap();
            });

            Processor.SubscribeReusable<Packets.BuildingData>((data) =>
            {
                data.Build();
            });
        }

        private void Stop()
        {
            Manager.Stop();
        }

        private void Update()
        {
            Manager.PollEvents();

            if (closeConnection)
                Stop();
        }
    }
}