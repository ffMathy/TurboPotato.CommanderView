using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetCoreServer;

namespace Server.Game
{
    public enum MapTileType
    {
        EmptySpace = 0,
        SolidWall = 1
    }

    public struct Player
    {
        public Guid Id { get; set; }
        public PlayerPosition Position { get; set; }
    }

    public struct PlayerPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class GameServer : TcpServer
    {
        public GameServer(string address, int port) : base(address, port)
        {
        }

        protected override TcpSession CreateSession()
        {
            var session = new GameServerSession(this);

            return session;
        }

        protected override void OnConnected(TcpSession session)
        {
            //TODO: somehow send map tiles to the connected client.

            //TODO: somehow send the client's ID (currentGameServerSession.Id) to the client, so he knows who he is.

            //TODO: somehow send a message to all other clients that a player has joined, and attach the player's initial position.
        }

        protected override void OnDisconnected(TcpSession session)
        {
            //TODO: send a message to all players that the player with the ID session.Id has disconnected.
        }
    }

    public class GameServerSession : TcpSession
    {
        private readonly GameServer _server;

        public GameServerSession(GameServer server) : base(server)
        {
            _server = server;
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"Client {Id} has connected to server.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client {Id} has disconnected from server.");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var text = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);

            //TODO: handle client position updates, and broadcast the position to all other clients.
        }
    }
}
