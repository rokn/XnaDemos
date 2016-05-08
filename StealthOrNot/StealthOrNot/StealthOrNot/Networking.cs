using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StealthOrNot
{
    public enum Packets
    {
        CONNECT,
        START,
        MOVE,
        ROTATEHAND,
        CHANGEDIR,
        ATTACK,
        JUMP,
        NEWLASER,
        RELASER,
        THROW,
        STEALTH,
        CLIMBDOWN
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
        private static NetConnection enemyConnection;

        private static NetIncomingMessage inc;

        private static double oldPlayerHorizontalSpeed;
        private static float oldPlayerHandRotation;
        private static int oldPlayerDir;
        private static List<Player> allPlayers;

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
            outMsg.Write(Main.MainPlayer is PoliceMan);
            GUI.AddMessage("Waiting for approval", Color.White, false, 500);
            IsInitialized = true;
            oldPlayerHorizontalSpeed = 0;
            oldPlayerHandRotation = 0f;
            oldPlayerDir = 1;
            Peer = Client;
            allPlayers = new List<Player>();
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
            GUI.AddMessage("Server started", Color.White, false, 1000);
            IsInitialized = true;
            oldPlayerHorizontalSpeed = 0;
            oldPlayerHandRotation = 0f;
            oldPlayerDir = 1;
            Peer = Server;
            allPlayers = new List<Player>();
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

            if (Main.HasStarted)
            {
                UpdateData();
            }
        }

        private static void UpdateData()
        {
            if (oldPlayerHorizontalSpeed != Main.MainPlayer.horizontalSpeed)
            {
                SendMove(Main.MainPlayer);
            }

            oldPlayerHorizontalSpeed = Main.MainPlayer.horizontalSpeed;

            if (oldPlayerHandRotation != Main.MainPlayer.handRotation)
            {
                SendHandRotate(Main.MainPlayer);
            }

            oldPlayerHandRotation = Main.MainPlayer.handRotation;

            if (oldPlayerDir != Main.MainPlayer.dir)
            {
                SendDirection(Main.MainPlayer);
            }

            oldPlayerDir = Main.MainPlayer.dir;
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

                    foreach (var player in allPlayers)
                    {
                        if (player.Name != exceptName)
                        {
                            Connections.Add(player.Connection);
                        }
                    }

                    Server.SendMessage(outMsg, Connections, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
            else
            {
                Client.SendMessage(outMsg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        private static void SendMessage(NetOutgoingMessage outMsg, string receiverName)
        {
            foreach (var player in allPlayers)
            {
                if (player.Name == receiverName)
                {
                    Server.SendMessage(outMsg, player.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }
        }

        private static void HandleClientIncomingData()
        {
            Packets packet = (Packets)inc.ReadByte();

            switch (packet)
            {
                case Packets.CONNECT:
                    Main.MainPlayer.Name = inc.ReadString();
                    break;

                case Packets.START:
                    Main.HasStarted = true;
                    Main.LightsOn = true;
                    int mainCount = inc.ReadInt32();

                    for (int i = 0; i < mainCount; i++)
                    {
                        string name = inc.ReadString();

                        if (name != Main.MainPlayer.Name)
                        {
                            Player player;

                            if (Main.MainPlayer is PoliceMan)
                            {
                                player = new PoliceMan(Main.HelicopterStartPosition + Main.PoliceManOffset, false, name, null);
                            }
                            else
                            {
                                player = new Stealther(new Vector2(3000, 1400), false, name, null);
                            }

                            Main.mainPlayers.Add(player);
                            allPlayers.Add(player);
                        }
                    }

                    int enemyCount = inc.ReadInt32();

                    for (int i = 0; i < enemyCount; i++)
                    {
                        string name = inc.ReadString();

                        if (name != Main.MainPlayer.Name)
                        {
                            Player player;

                            if (Main.MainPlayer is PoliceMan)
                            {
                                player = new Stealther(new Vector2(3000, 1400), false, name, null);
                            }
                            else
                            {
                                player = new PoliceMan(Main.HelicopterStartPosition + Main.PoliceManOffset, false, name, null);
                            }

                            Main.enemyPlayers.Add(player);
                            allPlayers.Add(player);
                        }
                    }

                    Helicopter.Initialize(Main.HelicopterStartPosition, new Vector2(150, Main.HelicopterStartPosition.Y));
                    break;

                default:
                    HandlePacket(packet);
                    break;
            }
        }

        private static void HandleServerIncomingData()
        {
            HandlePacket((Packets)inc.ReadByte());
        }

        private static void HandlePacket(Packets packet)
        {
            Player player = FindPlayer(inc.ReadString());

            if (player != null)
            {
                switch (packet)
                {
                    case Packets.MOVE:
                        player.horizontalSpeed = inc.ReadDouble();
                        player.UpdatePosition(inc.ReadVector());

                        if (IsHost)
                            SendMove(player);
                        break;

                    case Packets.ROTATEHAND:
                        player.handRotation = inc.ReadFloat();
                        player.UpdateHand();

                        if (IsHost)
                            SendHandRotate(player);
                        break;

                    case Packets.CHANGEDIR:
                        player.dir = inc.ReadInt32();

                        if (IsHost)
                            SendDirection(player);
                        break;

                    case Packets.ATTACK:
                        player.Attack();

                        if (IsHost)
                            SendAttack(player);
                        break;

                    case Packets.JUMP:
                        player.verticalSpeed = inc.ReadDouble();
                        player.isOnGround = false;
                        player.CheckForGround = false;
                        player.UpdatePosition(inc.ReadVector());

                        if (IsHost)
                            SendJump(player);
                        break;

                    case Packets.NEWLASER:
                        Vector2 pos = inc.ReadVector();
                        Main.lasers.Add(new Laser(pos));

                        if (IsHost)
                            SendLaser(player, pos);
                        break;

                    case Packets.RELASER:
                        int id = inc.ReadInt32();
                        Laser laser = Main.lasers.ElementAtOrDefault(id);

                        if (laser != null)
                        {
                            laser.IsActivated = true;
                        }

                        if (IsHost)
                            ReactivateLaser(id, player);
                        break;

                    case Packets.THROW:
                        player.Throw();

                        if (IsHost)
                            SendThrowable(player);
                        break;

                    case Packets.STEALTH:
                        (player as Stealther).ChangeStealth();

                        if (IsHost)
                            SendStealthChange(player);
                        break;

                    case Packets.CLIMBDOWN:
                        (player as PoliceMan).ClimbDown();

                        if (IsHost)
                            SendClimbDown(player);
                        break;
                }
            }
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
                            Console.WriteLine("Incoming login");
                            enemyConnection = inc.SenderConnection;
                            inc.SenderConnection.Approve();
                            Console.WriteLine("Approved new connection");

                            int id = allPlayers.Count + 1;
                            string name = "name" + id.ToString();

                            if (Main.MainPlayer is PoliceMan)
                            {
                                if (inc.ReadBoolean())
                                {
                                    PoliceMan player = new PoliceMan(Main.HelicopterStartPosition + Main.PoliceManOffset, false, name, inc.SenderConnection);
                                    Main.mainPlayers.Add(player);
                                    allPlayers.Add(player);
                                }
                                else
                                {
                                    Stealther player = new Stealther(new Vector2(3000, 1400), false, name, inc.SenderConnection);
                                    Main.enemyPlayers.Add(player);
                                    allPlayers.Add(player);
                                }
                            }
                            else
                            {
                                if (inc.ReadBoolean())
                                {
                                    PoliceMan player = new PoliceMan(Main.HelicopterStartPosition + Main.PoliceManOffset, false, name, inc.SenderConnection);
                                    Main.enemyPlayers.Add(player);
                                    allPlayers.Add(player);
                                }
                                else
                                {
                                    Stealther player = new Stealther(new Vector2(3000, 1400), false, name, inc.SenderConnection);
                                    Main.mainPlayers.Add(player);
                                    allPlayers.Add(player);
                                }
                            }

                            NetOutgoingMessage outMsg = Server.CreateMessage();
                            outMsg.Write((byte)Packets.CONNECT);
                            outMsg.Write(name);
                            SendMessage(outMsg, name);
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        HandleServerIncomingData();
                        break;
                }
            }
            if (Main.HasStarted)
            {
                UpdateData();
            }
        }

        private static void SendDirection(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.CHANGEDIR);
            outMsg.Write(player.Name);
            outMsg.Write(player.dir);
            SendMessage(outMsg, player.Name, false);
        }

        public static void SendAttack(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.ATTACK);
            outMsg.Write(player.Name);
            SendMessage(outMsg, player.Name, false);
        }

        public static void SendJump(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.JUMP);
            outMsg.Write(player.Name);
            outMsg.Write(player.verticalSpeed);
            outMsg.Write(player.Position);
            SendMessage(outMsg, player.Name, false);
        }

        public static void SendLaser(Player player, Vector2 position)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.NEWLASER);
            outMsg.Write(player.Name);
            outMsg.Write(position);
            SendMessage(outMsg, player.Name, false);
        }

        private static void SendMove(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.MOVE);
            outMsg.Write(player.Name);
            outMsg.Write(player.horizontalSpeed);
            outMsg.Write(player.Position);
            SendMessage(outMsg, player.Name, false);
        }

        private static void SendHandRotate(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.ROTATEHAND);
            outMsg.Write(player.Name);
            outMsg.Write(player.handRotation);
            SendMessage(outMsg, player.Name, false);
        }

        public static void SendThrowable(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.THROW);
            outMsg.Write(player.Name);
            SendMessage(outMsg, player.Name, false);
        }

        public static void SendStealthChange(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.STEALTH);
            outMsg.Write(player.Name);
            SendMessage(outMsg, player.Name, false);
        }

        public static void Start()
        {
            foreach (Player player in Main.enemyPlayers)
            {
                NetOutgoingMessage outMsg = Server.CreateMessage();
                outMsg.Write((byte)Packets.START);
                outMsg.Write(Main.enemyPlayers.Count);

                foreach (Player p in Main.enemyPlayers)
                {
                    outMsg.Write(p.Name);
                }

                outMsg.Write(Main.mainPlayers.Count);

                foreach (Player p in Main.mainPlayers)
                {
                    outMsg.Write(p.Name);
                }

                Server.SendMessage(outMsg, player.Connection, NetDeliveryMethod.ReliableOrdered, 0);
            }

            foreach (Player player in Main.mainPlayers)
            {
                if (player.Name != Main.MainPlayer.Name)
                {
                    NetOutgoingMessage outMsg = Server.CreateMessage();
                    outMsg.Write((byte)Packets.START);
                    outMsg.Write(Main.mainPlayers.Count);

                    foreach (Player p in Main.mainPlayers)
                    {
                        outMsg.Write(p.Name);
                    }

                    outMsg.Write(Main.enemyPlayers.Count);

                    foreach (Player p in Main.enemyPlayers)
                    {
                        outMsg.Write(p.Name);
                    }

                    Server.SendMessage(outMsg, player.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                }
            }

            Main.HasStarted = true;
            Main.LightsOn = true;
            Helicopter.Initialize(Main.HelicopterStartPosition, new Vector2(150, Main.HelicopterStartPosition.Y));
        }

        private static Player FindPlayer(string name)
        {
            foreach (Player p in allPlayers)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }

        public static void ReactivateLaser(int laserId, Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.RELASER);
            outMsg.Write(player.Name);
            outMsg.Write(laserId);
            SendMessage(outMsg, player.Name, false);
        }

        internal static void SendClimbDown(Player player)
        {
            NetOutgoingMessage outMsg = Peer.CreateMessage();
            outMsg.Write((byte)Packets.CLIMBDOWN);
            outMsg.Write(player.Name);
            SendMessage(outMsg, player.Name, false);
        }
    }
}