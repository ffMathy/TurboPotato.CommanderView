using System.Linq;

namespace CommanderView.Server.Game
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
                .Aggregate((a, b) => a + ";");

            var updateMapCommandString = $"UPDATEMAP:{mapString}";
            return updateMapCommandString;
        }
    }
}