using System;
using Lidgren.Network;
using System.Collections.Generic;

namespace TankServer
{
    public enum Packets
    {
        Login,
        Move,
        Start,
        Shoot
    }

    class main
    {

        static NetServer server;

        static NetPeerConfiguration config;

        static int terrainWidth;

        static List<Player> players;
        
        static void Main()
        {
            config = new NetPeerConfiguration("Tanks");
            config.Port = 14242;
            config.MaximumConnections = 2;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            players = new List<Player>();
            server = new NetServer(config);

            server.Start();
            Console.WriteLine("Server started");

            NetIncomingMessage incMsg;

            while(true)
            {
                if((incMsg = server.ReadMessage())!=null)
                {
                    switch(incMsg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:

                            incMsg.SenderConnection.Approve();
                            Console.WriteLine("Connection "+incMsg.SenderConnection.ToString()+" approved");
                            players.Add(new Player(incMsg.SenderConnection));
                            Console.WriteLine(players.Count);
                            if(incMsg.ReadByte()==(byte)Packets.Login)
                            {
                                terrainWidth = incMsg.ReadInt32();
                            }
                            if(players.Count==2)
                            {
                                StartGame();
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            switch((Packets)incMsg.ReadByte())
                            {
                                case Packets.Move:
                                    foreach (Player player in players)
                                    {
                                        if (player.connection == incMsg.SenderConnection)
                                        {
                                            player.X = incMsg.ReadInt32();
                                            UpdatePositions();
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private static void StartGame()
        {
            Random rand = new Random();
            players[0].X = rand.Next(200);
            players[1].X = rand.Next(terrainWidth - 200, terrainWidth - 1);
            byte p = 0;
            foreach (Player player in players)
            {
                NetOutgoingMessage outMsg = server.CreateMessage();
                outMsg.Write((byte)Packets.Start);
                outMsg.Write(p);
                outMsg.Write(players[0].X);
                outMsg.Write(players[1].X);
                p++;
                server.SendMessage(outMsg, player.connection, NetDeliveryMethod.ReliableOrdered);
            }
            Console.WriteLine("Game Started");
        }

        private static void UpdatePositions()
        {
            byte p = 0;
            foreach (Player player in players)
            {
                NetOutgoingMessage outMsg = server.CreateMessage();
                outMsg.Write((byte)Packets.Move);
                outMsg.Write(p);
                outMsg.Write(players[0].X);
                outMsg.Write(players[1].X);
                p++;
                server.SendMessage(outMsg, player.connection, NetDeliveryMethod.ReliableOrdered);
            }
        }
    }

    public class Player
    {
        public int X;
        public int Y;
        public NetConnection connection;
        public Player(NetConnection Connection)
        {
            this.connection = Connection;
            X = 0;
            Y = 0;
        }
    }
}
