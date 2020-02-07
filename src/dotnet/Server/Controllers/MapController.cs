using Microsoft.AspNetCore.Mvc;
using Server.Game;
using System;

namespace Server.Controllers
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
        public IActionResult GetMap()
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpPost("")]
        public IActionResult UpdateMap([FromBody] MapUpdateRequest request)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
