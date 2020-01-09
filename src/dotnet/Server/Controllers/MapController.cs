using Microsoft.AspNetCore.Mvc;
using Server.Game;

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
            return Ok(new MapResponse()
            {
                TileMatrix = _gameServer.MapTileMatrix,
                Players = _gameServer.GetPlayers()
            });
        }

        [HttpPost("")]
        public IActionResult UpdateMap([FromBody] MapUpdateRequest request)
        {
            _gameServer.SetMapTileMatrix(request.TileMatrix);
            return NoContent();
        }
    }
}
