using System;
using System.Linq;

namespace Server.Game
{
    public class CommandStringHelper
    {
        public static string GetPlayerUpdateCommandStringFromGameServerSession(GameServerSession gameServerSession)
        {
            return $"PLAYERUPDATE:{gameServerSession.Id};{gameServerSession.PlayerPosition.X};{gameServerSession.PlayerPosition.Y}";
        }

        public static string GetUpdateMapCommandStringFromTileMatrix(MapTileType[][] tileMatrix)
        {
            var mapString = tileMatrix
                .Select(tileRow => tileRow
                    .Select(tileTypeEnum => (int)tileTypeEnum)
                    .Select(tileTypeNumber => tileTypeNumber.ToString())
                    .Aggregate((a, b) => a + "" + b))
                .Aggregate((a, b) => a + ";" + b);

            var updateMapCommandString = $"UPDATEMAP:{mapString}";
            return updateMapCommandString;
        }

        public static string InsertEndOfCommandMarker(string input)
        {
            return input + "\0";
        }

        public static string[] SplitReceivedTextByEndOfCommandMarkers(string input)
        {
            return input.Split('\0', StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetClientIdCommandStringFromClientId(Guid sessionId)
        {
            return "ID:" + sessionId;
        }
    }
}