using System.Threading;

using UnityEngine;

using LiteNetLib;
using LiteNetLib.Utils;

namespace VoxelMultiplayer.Network
{
    public class ServerPeer : MonoBehaviour
    {
        private static EventBasedNetListener Listener;
        public static NetManager Manager { get; private set; }
        public static NetPacketProcessor Processor { get; private set; }

        private readonly int Port = 23020;
        private readonly int maxConnected = 1;
        private readonly string Key = "";

        public bool closeConnection = false;
        private static byte[] CurrentMapData { get; set; }

        private void Start()
        {
            Debug.LogWarning("Server.Start(): Starting Local Server");

            Listener = new EventBasedNetListener();
            Manager = new NetManager(Listener);
            Processor = new NetPacketProcessor();

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

                Processor.Send(peer, new Packets.MapData() { Length = CurrentMapData.Length, Data = CurrentMapData }, DeliveryMethod.ReliableOrdered);
            };

            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Processor.ReadAllPackets(dataReader, fromPeer);

                dataReader.Recycle();
            };

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

        public static bool ReceiveLatestMap(byte[] data) { if (data != null) { CurrentMapData = data; return true; } else return false; }
    }
}