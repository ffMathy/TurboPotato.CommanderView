using System;
using System.Text;
using System.Threading.Tasks;
using NetCoreServer;

namespace SimpleGameClient
{
    class Program
    {
        private static readonly GameClient _client;

        static Program()
        {
            _client = new GameClient("127.0.0.1", 14567);
        }

        static async Task Main()
        {
            if (!_client.ConnectAsync())
                throw new InvalidOperationException("Could not connect.");

            while(true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                Console.WriteLine("Sending: " + line);
                SendNullTerminated(line);
            }
        }

        static void SendNullTerminated(string text)
        {
            _client.SendAsync(text + "\0");
        }
    }

    class GameClient : TcpClient
    {
        private string _id;

        public GameClient(string address, int port) : base(address, port)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Client connected.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine("Client disconnected.");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var text = Encoding.UTF8.GetString(buffer, (int) offset, (int) size);
            var commands = text.Split('\0', StringSplitOptions.RemoveEmptyEntries);
            foreach (var command in commands)
            {
                Console.WriteLine("Received: " + command);

                var split = command.Split(':');

                var commandName = split[0];
                var commandArguments = split[1].Split(';');

                HandleReceivedCommand(commandName, commandArguments);
            }
        }

        private void HandleReceivedCommand(string commandName, string[] commandArguments)
        {
            switch(commandName)
            {
                case "ID":
                {
                    _id = Console.Title = commandArguments[0];
                    break;
                }
            }
        }
    }
}
