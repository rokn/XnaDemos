using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG
{
    public enum Packets
    {
        CONNECT,
    }

    public static class Networking
    {
        private static NetPeer Peer;
        private static NetServer Server;
        private static NetClient Client;
        private static NetPeerConfiguration ConfigServer;
        private static NetPeerConfiguration ConfigClient;
        public static bool IsHost;
        public static bool IsInitialized;
        private static string hostIp;
        private static NetIncomingMessage inc;

        public static void InitializeClient(string ip)
        {
            ConfigClient = new NetPeerConfiguration("StealtOrNot");
            IsHost = false;
            hostIp = ip;
            Client = new NetClient(ConfigClient);
            NetOutgoingMessage outMsg = Client.CreateMessage();
            Client.Start();
            outMsg.Write((byte)Packets.CONNECT);
            Client.Connect(hostIp, 14242, outMsg);
            Peer = Client;
        }

        public static void InitializeServer()
        {
            ConfigServer = new NetPeerConfiguration("StealtOrNot");
            ConfigServer.Port = 14242;
            IsHost = true;
            ConfigServer.MaximumConnections = 7;
            ConfigServer.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(ConfigServer);
            Server.Start();
            Peer = Server;
        }

        public static void Update()
        {
            if (IsInitialized)
            {
                if (IsHost)
                {
                    ServerUpdate();
                }
                else
                {
                    ClientUpdate();
                }
            }
        }

        private static void ClientUpdate()
        {
            while ((inc = Client.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        HandleClientIncomingData();
                        break;
                }
            }
        }

        private static void SendMessage(NetOutgoingMessage outMsg, string exceptName, bool sendToAll = false)
        {
            if (IsHost)
            {
                if (sendToAll)
                {
                    Server.SendMessage(outMsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                }
                else
                {
                    List<NetConnection> Connections = new List<NetConnection>();

                    Server.SendMessage(outMsg, Connections, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
            else
            {
                Client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
        private static void HandleClientIncomingData()
        {
            Packets packet = (Packets)inc.ReadByte();
        }

        private static void HandleServerIncomingData()
        {
        }

        private static void ServerUpdate()
        {
            //if (Main.HasStarted)
            //{
            //    if (Server.ConnectionsCount < 1)
            //    {
            //        Main.HasStarted = false;                                                       //TODO : Implement this
            //        GUI.AddMessage("Opponent disconnected", Color.White, false, 1500);             //                          <------
            //    }
            //}

            while ((inc = Server.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        if (inc.ReadByte() == (byte)Packets.CONNECT)
                        {                            

                            NetOutgoingMessage outMsg = Server.CreateMessage();
                            outMsg.Write((byte)Packets.CONNECT);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        HandleServerIncomingData();
                        break;
                }
            }
        }
    }
}