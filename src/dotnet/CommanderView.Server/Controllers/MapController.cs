using System;
using System.Collections.Generic;
using System.Linq;
using CommanderView.Server.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommanderView.Server.Controllers
{
    public class MapResponse
    {
        public MapTileType[][] TileMatrix { get; set; }
        public Player[] Players { get; set; }
    }

    public class MapUpdateRequest
    {
        public MapTileType[][] TileMatrix { get; set; }
    }

    [ApiController]
    [Route("api/map")]
    public class MapController : ControllerBase
    {
        private readonly GameServer _gameServer;

        public MapController(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        [HttpGet("")]
        public MapResponse GetMap()
        {
            return new MapResponse()
            {
                TileMatrix = _gameServer.MapTileMatrix,
                Players = _gameServer.GetPlayers()
            };
        }

        [HttpPost("")]
        public void UpdateMap(MapUpdateRequest request)
        {
            _gameServer.SetMapTileMatrix(request.TileMatrix);
        }
    }
}
