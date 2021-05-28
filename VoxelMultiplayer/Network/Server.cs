using System;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using LiteNetLib;
using LiteNetLib.Utils;

namespace VoxelMultiplayer.Network
{
    public class Server
    {
        private static EventBasedNetListener Listener;
        private static NetManager Manager;

        private readonly int Port = 23020;
        private readonly int maxConnected = 1;
        private readonly string Key = "";

        public bool closeConnection = false;

        //private string currentMapData { get; set; }
        private byte[] CurrentMapData { get; set; }

        public void Start()
        {
            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener);

            Manager.Start(Port);

            Listener.ConnectionRequestEvent += request =>
            {
                if (Manager.ConnectedPeersCount < maxConnected /* max connections */)
                    request.AcceptIfKey(Key);
                else
                    request.Reject();
            };

            Listener.PeerConnectedEvent += peer =>
            {
                Debug.LogWarning("We got connection: " + peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                //writer.Put("Hello client!");                                // Put some string

                //Debug.LogError(currentMapData);
                writer.Put(CurrentMapData.Length);
                writer.Put(CurrentMapData);

                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };

            while (true)
            {
                Manager.PollEvents();
                Thread.Sleep(15);

                if (closeConnection)
                    break;
            }

            Manager.Stop();
        }

        public void Stop()
        {
            closeConnection = true;
        }

        public bool ReceiveLatestMap(byte[] data) { if (data != null) { CurrentMapData = data; return true; } else return false; }
    }
}