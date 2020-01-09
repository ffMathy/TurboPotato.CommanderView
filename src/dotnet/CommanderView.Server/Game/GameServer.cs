using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using NetCoreServer;

namespace CommanderView.Server.Game
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
        private readonly Dictionary<Guid, GameServerSession> _gameServerSessionsById;

        public MapTileType[][] MapTileMatrix { get; private set; }

        public GameServer(string address, int port) : base(address, port)
        {
            _gameServerSessionsById = new Dictionary<Guid, GameServerSession>();

            MapTileMatrix = GenerateInitialMapTileMatrix();
        }

        protected override TcpSession CreateSession()
        {
            var session = new GameServerSession(this);
            _gameServerSessionsById.Add(session.Id, session);

            return session;
        }

        protected override void OnConnected(TcpSession session)
        {
            var currentGameServerSession = _gameServerSessionsById[session.Id];

            //send a message to all other clients that a player has joined, and attach the player's initial position.
            MulticastLogged(CommandStringHelper.GetPlayerUpdateCommandStringFromGameServerSession(currentGameServerSession));

            //send map tiles to the connected client.
            currentGameServerSession.SendLogged(CommandStringHelper.GetUpdateMapCommandStringFromTileMatrix(MapTileMatrix));

            //send other already joined player positions to the connected client.
            foreach(var otherGameServerSession in _gameServerSessionsById.Values)
            {
                currentGameServerSession.SendLogged(CommandStringHelper.GetPlayerUpdateCommandStringFromGameServerSession(otherGameServerSession));
            }
        }

        protected override void OnDisconnected(TcpSession session)
        {
            _gameServerSessionsById.Remove(session.Id);

            MulticastLogged($"PLAYERLEFT:{session.Id}");
        }

        public Player[] GetPlayers()
        {
            return _gameServerSessionsById.Values
                .Select(x => new Player()
                {
                    Id = x.Id,
                    Position = x.PlayerPosition
                })
                .ToArray();
        }

        public void SetMapTileMatrix(MapTileType[][] tileMatrix)
        {
            MapTileMatrix = tileMatrix;

            var updateMapCommandString = CommandStringHelper.GetUpdateMapCommandStringFromTileMatrix(tileMatrix);
            Console.WriteLine("Sending command to all clients: " + updateMapCommandString);

            MulticastLogged(updateMapCommandString);
        }

        public void MulticastLogged(string text)
        {
            Console.WriteLine($"Sending to all clients: {text}");
            Console.WriteLine();

            Multicast(text);
        }

        private static MapTileType[][] GenerateInitialMapTileMatrix()
        {
            return new[]
            {
                new [] { MapTileType.SolidWall, MapTileType.SolidWall, MapTileType.SolidWall, MapTileType.SolidWall },
                new [] { MapTileType.SolidWall, MapTileType.EmptySpace, MapTileType.EmptySpace, MapTileType.SolidWall },
                new [] { MapTileType.SolidWall, MapTileType.EmptySpace, MapTileType.EmptySpace, MapTileType.SolidWall },
                new [] { MapTileType.SolidWall, MapTileType.SolidWall, MapTileType.SolidWall, MapTileType.SolidWall }
            };
        }
    }

    public class GameServerSession : TcpSession
    {
        private readonly GameServer _server;

        public PlayerPosition PlayerPosition { get; private set;  }

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
            Console.WriteLine($"{Id} to server: {text}");

            HandleReceivedPositionUpdateCommand(text);
        }

        public void SendLogged(string text)
        {
            Console.WriteLine($"Sending to client {Id}: {text}");
            Console.WriteLine();

            Send(text);
        }

        private void HandleReceivedPositionUpdateCommand(string text)
        {
            const string positionUpdateCommand = "POSITION-UPDATE:";

            if (!text.StartsWith(positionUpdateCommand)) 
                return;

            //parse the position X and Y coordinates from the command.
            var commandParameterText = text.Substring(positionUpdateCommand.Length);
            var coordinateSplit = commandParameterText.Split(";");

            var playerPosition = new PlayerPosition()
            {
                X = double.Parse(coordinateSplit[0], Program.UnitedStatesCulture),
                Y = double.Parse(coordinateSplit[1], Program.UnitedStatesCulture)
            };

            PlayerPosition = playerPosition;

            //broadcast a player update to all the server's clients.
            _server.MulticastLogged(CommandStringHelper.GetPlayerUpdateCommandStringFromGameServerSession(this));
        }
    }
}
