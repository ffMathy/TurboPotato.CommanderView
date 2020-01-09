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

            //send map tiles to the connected client.
            currentGameServerSession.SendNullTerminated(CommandStringHelper.GetUpdateMapCommandStringFromTileMatrix(MapTileMatrix));

            //send the client's ID to the client.
            currentGameServerSession.SendNullTerminated(CommandStringHelper.GetClientIdCommandStringFromClientId(session.Id));

            //send a message to all other clients that a player has joined, and attach the player's initial position.
            MulticastNullTerminated(CommandStringHelper.GetPlayerUpdateCommandStringFromGameServerSession(currentGameServerSession));
        }

        protected override void OnDisconnected(TcpSession session)
        {
            _gameServerSessionsById.Remove(session.Id);

            MulticastNullTerminated($"PLAYERLEFT:{session.Id}");
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

            //broadcast the new map to all clients.
            MulticastNullTerminated(CommandStringHelper.GetUpdateMapCommandStringFromTileMatrix(tileMatrix));
        }

        public void MulticastNullTerminated(string text)
        {
            Console.WriteLine($"Sending to all clients: {text}");
            Multicast(CommandStringHelper.InsertEndOfCommandMarker(text));
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
            var commands = CommandStringHelper.SplitReceivedTextByEndOfCommandMarkers(text);
            foreach (var command in commands)
            {
                Console.WriteLine($"Received from client {Id}: {command}");

                var split = command.Split(':');

                var commandName = split[0];
                var commandArguments = split?.Length > 1 ? 
                    split[1].Split(';') : 
                    Array.Empty<string>();

                HandleReceivedCommand(commandName, commandArguments);
            }
        }

        public void SendNullTerminated(string text)
        {
            Console.WriteLine($"Sending to client {Id}: {text}");
            Send(CommandStringHelper.InsertEndOfCommandMarker(text));
        }

        private void HandleReceivedCommand(string commandName, string[] commandArguments)
        {
            switch (commandName)
            {
                case "POSITIONUPDATE":
                {
                    //parse the position X and Y coordinates from the command.
                    var playerPosition = new PlayerPosition()
                    {
                        X = double.Parse(commandArguments[0], Program.UnitedStatesCulture),
                        Y = double.Parse(commandArguments[1], Program.UnitedStatesCulture)
                    };

                    PlayerPosition = playerPosition;

                    //broadcast a player update to all the server's clients.
                    _server.MulticastNullTerminated(
                        CommandStringHelper.GetPlayerUpdateCommandStringFromGameServerSession(this));
                    break;
                }
            }
        }
    }
}
