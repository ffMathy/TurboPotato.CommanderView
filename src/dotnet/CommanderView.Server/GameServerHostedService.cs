using System;
using System.Threading;
using System.Threading.Tasks;
using CommanderView.Server.Game;
using Microsoft.Extensions.Hosting;

namespace CommanderView.Server
{
    public class GameServerHostedService : IHostedService
    {
        private readonly GameServer _gameServer;

        public GameServerHostedService(
            GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_gameServer.Start())
                throw new InvalidOperationException("Could not start server.");

            Console.WriteLine($"Server listening for game clients at port {_gameServer.Endpoint.Address}:{_gameServer.Endpoint.Port}.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if(!_gameServer.Stop())
                throw new InvalidOperationException("Could not stop server.");

            Console.WriteLine($"Server stopped listening for game clients.");
        }
    }
}