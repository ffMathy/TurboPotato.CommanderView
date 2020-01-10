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
                /* TODO: the response should contain the map tile matrix and the players on the map.
                 * perhaps it should be fetched from the GameServer? */
            });
        }

        [HttpPost("")]
        public IActionResult UpdateMap([FromBody] MapUpdateRequest request)
        {
            /* TODO: we should update the current map tile matrix (perhaps on the GameServer?) with the map given in the request. */
            /* TODO: we should also (via the server) broadcast the updated map to all clients. */

            return NoContent();
        }
    }
}
