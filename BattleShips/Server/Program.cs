using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;



// Lidgren Network example
// Made by: Riku Koskinen
// http://xnacoding.blogspot.com/
// Download LidgreNetwork at: http://code.google.com/p/lidgren-network-gen3/
//
// You can use this code in anyway you want
// Code is not perfect, but it works
// It's example of console based game, where new players can join and move
// Movement is updated to all clients.


// THIS IS VERY VERY VERY BASIC EXAMPLE OF NETWORKING IN GAMES
// NO PREDICTION, NO LAG COMPENSATION OF ANYKIND

namespace Server
{
    class Program
    {
        // Server object
        static NetServer Server;
        // Configuration object
        static NetPeerConfiguration Config;

        static void Main(string[] args)
        {
            int begins=0;
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            Config = new NetPeerConfiguration("game");

            // Set server port
            Config.Port = 14242;

            // Max client amount
            Config.MaximumConnections = 200;

            // Enable New messagetype. Explained later
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Create new server based on the configs just defined
            Server = new NetServer(Config);

            // Start it
            Server.Start();

            // Eh..
            Console.WriteLine("Server Started");

            // Create list of "Characters" ( defined later in code ). This list holds the world state. Character positions
            List<Character> GameWorldState = new List<Character>();

            // Object that can be used to store and read messages
            NetIncomingMessage inc;

            // Check time
            DateTime time = DateTime.Now;

            // Create timespan of 30ms
            TimeSpan timetopass = new TimeSpan(0, 0, 0, 0, 30);

            // Write to con..
            Console.WriteLine("Waiting for new connections and updateing world state to current ones");

            // Main loop
            // This kind of loop can't be made in XNA. In there, its basically same, but without while
            // Or maybe it could be while(new messages)
            while (true)
            {
                // Server.ReadMessage() Returns new messages, that have not yet been read.
                // If "inc" is null -> ReadMessage returned null -> Its null, so dont do this :)
                if ((inc = Server.ReadMessage()) != null)
                {
                    // Theres few different types of messages. To simplify this process, i left only 2 of em here
                    switch (inc.MessageType)
                    {
                        // If incoming message is Request for connection approval
                        // This is the very first packet/message that is sent from client
                        // Here you can do new player initialisation stuff
                        case NetIncomingMessageType.ConnectionApproval:

                            // Read the first byte of the packet
                            // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                            if (inc.ReadByte() == (byte)PacketTypes.LOGIN)
                            {
                                Console.WriteLine("Incoming LOGIN");

                                // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                                inc.SenderConnection.Approve();

                                // Init random
                                Random r = new Random();

                                // Add new character to the game.
                                // It adds new player to the list and stores name, ( that was sent from the client )
                                // Random x, y and stores client IP+Port
                                GameWorldState.Add(new Character(inc.ReadString(), inc.SenderConnection));

                                // Create message, that can be written and sent
                                NetOutgoingMessage outmsg = Server.CreateMessage();

                                // first we write byte
                                outmsg.Write((byte)PacketTypes.WORLDSTATE);

                                // then int
                                outmsg.Write(GameWorldState.Count);

                                // iterate trought every character ingame
                                foreach (Character ch in GameWorldState)
                                {
                                    // This is handy method
                                    // It writes all the properties of object to the packet
                                    outmsg.WriteAllProperties(ch);
                                }

                                // Now, packet contains:
                                // Byte = packet type
                                // Int = how many players there is in game
                                // character object * how many players is in game

                                // Send message/packet to all connections, in reliably order, channel 0
                                // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                                Server.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                                // Debug
                                Console.WriteLine("Approved new connection and updated the world status");
                            }

                            break;
                        // Data type is all messages manually sent from client
                        // ( Approval is automated process )
                        case NetIncomingMessageType.Data:
                            switch (inc.ReadByte())
                            {
                                case (byte)PacketTypes.BEGIN:
                                    begins++;
                                    if (begins >= 2)
                                    {
                                        bool turn = true;
                                        foreach (Character ch in GameWorldState)
                                        {
                                            NetOutgoingMessage outmsg = Server.CreateMessage();
                                            outmsg.Write((byte)PacketTypes.BEGIN);
                                            outmsg.Write(turn);
                                            turn = !turn;
                                            Server.SendMessage(outmsg, ch.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                                        }
                                    }
                                    break;
                                case (byte)PacketTypes.ATTACK:
                                    foreach (Character ch in GameWorldState)
                                    {
                                        if (inc.SenderConnection != ch.Connection)
                                        {
                                            NetOutgoingMessage outmsg = Server.CreateMessage();
                                            outmsg.Write((byte)PacketTypes.ATTACK);
                                            int x = inc.ReadInt32(), y = inc.ReadInt32();
                                            outmsg.Write(x);
                                            outmsg.Write(y);
                                            Server.SendMessage(outmsg, ch.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                                        }
                                    }
                                    break;
                                case (byte)PacketTypes.RETURN:
                                    foreach (Character ch in GameWorldState)
                                    {
                                        if (inc.SenderConnection != ch.Connection)
                                        {
                                            NetOutgoingMessage outmsg = Server.CreateMessage();
                                            outmsg.Write((byte)PacketTypes.RETURN);
                                            outmsg.Write(inc.ReadBoolean());
                                            Server.SendMessage(outmsg, ch.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                                        }
                                    }
                                    break;
                            }
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            // In case status changed
                            // It can be one of these
                            // NetConnectionStatus.Connected;
                            // NetConnectionStatus.Connecting;
                            // NetConnectionStatus.Disconnected;
                            // NetConnectionStatus.Disconnecting;
                            // NetConnectionStatus.None;

                            // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                            Console.WriteLine(inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status);
                            if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                // Find disconnected character and remove it
                                foreach (Character cha in GameWorldState)
                                {
                                    if (cha.Connection == inc.SenderConnection)
                                    {
                                        GameWorldState.Remove(cha);
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            // As i stated previously, theres few other kind of messages also, but i dont cover those in this example
                            // Uncommenting next line, informs you, when ever some other kind of message is received
                            //Console.WriteLine("Not Important Message");
                            break;
                    }
                } // If New messages

                // if 30ms has passed
                if ((time + timetopass) < DateTime.Now)
                {
                    // If there is even 1 client
                    if (Server.ConnectionsCount != 0)
                    {
                        // Create new message
                        NetOutgoingMessage outmsg = Server.CreateMessage();

                        // Write byte
                        outmsg.Write((byte)PacketTypes.WORLDSTATE);

                        // Write Int
                        outmsg.Write(GameWorldState.Count);

                        // Iterate throught all the players in game
                        foreach (Character ch2 in GameWorldState)
                        {

                            // Write all properties of character, to the message
                            outmsg.WriteAllProperties(ch2);
                        }

                        // Message contains
                        // byte = Type
                        // Int = Player count
                        // Character obj * Player count

                        // Send messsage to clients ( All connections, in reliable order, channel 0)
                        Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    // Update current time
                    time = DateTime.Now;
                }

                // While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                //System.Threading.Thread.Sleep(1);
            }
        }
    }


    /// <summary>
    /// Character class
    /// 
    /// This class is passed around.
    /// It holds the position, name ( not used in this example ) ( even thou it gets sent all over )
    /// Connection (ip+port)
    /// 
    /// </summary>
    class Character
    {
        public string Name { get; set; }
        public NetConnection Connection { get; set; }
        public Character(string name, NetConnection conn)
        {
            Name = name;
            Connection = conn;
        }
    }


    // This is good way to handle different kind of packets
    // there has to be some way, to detect, what kind of packet/message is incoming.
    // With this, you can read message in correct order ( ie. Can't read int, if its string etc )

    // Best thing about this method is, that even if you change the order of the entrys in enum, the system won't break up
    // Enum can be casted ( converted ) to byte
    enum PacketTypes
    {
        LOGIN,
        BEGIN,
        WORLDSTATE,
        ATTACK,
        RETURN
    }
    //class LoginPacket
    //{
    //    public string MyName { get; set; }
    //    public LoginPacket(string name)
    //    {
    //        MyName = name;
    //    }
    //}

    // Movement directions
    // This way we can just send byte over net and no need to send anything bigger
}
