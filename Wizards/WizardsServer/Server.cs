using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
namespace WizardsServer
{
    public enum Packets
    {
        MOVE,
        UPDATE,
        CONNECT
    }
    class Server
    {
        public static NetServer server;
        public static NetPeerConfiguration config;
        static void Main()
        {
            config = new NetPeerConfiguration("Wizards");
            config.Port = 14242;
            config.MaximumConnections = 8;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            server = new NetServer(config);
            server.Start();

            List<Player> players = new List<Player>();

            NetIncomingMessage incomingMessage;

            DateTime time = DateTime.Now;

            TimeSpan updateTime = new TimeSpan(0, 0, 0, 0, 30);

            Console.WriteLine("Server started and waiting for connections");

            while(true)
            {
                if ((incomingMessage = server.ReadMessage()) != null)
                {
                    switch(incomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            Console.WriteLine("Incoming connection: "+incomingMessage.SenderConnection.ToString());
                            incomingMessage.SenderConnection.Approve();

                            players.Add(new Player(incomingMessage.ReadString(),new Vector2(),incomingMessage.SenderConnection));

                            NetOutgoingMessage outMessage = server.CreateMessage();

                            outMessage.Write((byte)Packets.CONNECT);
                            outMessage.Write(players.Count);
                            foreach(Player p in players)
                            {
                                outMessage.Write(p.Name);
                                outMessage.Write(p.Position.X);
                                outMessage.Write(p.Position.Y);
                            }
                            server.SendMessage(outMessage, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            break;
                        case NetIncomingMessageType.Data:
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine(incomingMessage.SenderConnection.ToString() + "status changed to: "+(NetConnectionStatus)incomingMessage.SenderConnection.Status);
                            if (incomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnected || incomingMessage.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                foreach (Player p in players)
                                {
                                    if (p.Connection == incomingMessage.SenderConnection)
                                    {
                                        players.Remove(p);
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if(time+updateTime < DateTime.Now)
                    {
                        time = DateTime.Now;
                    }
                }
            }
        }
    }
    class Player
    {
        public Vector2 Position;
        public string Name { get; set; }
        public NetConnection Connection { get; set; }
        public Player(string name, Vector2 pos, NetConnection conn)
        {
            Name = name;
            Position = pos;
            Connection = conn;
        }
    }
}
