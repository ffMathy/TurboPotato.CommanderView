using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetCoreServer;
using System.Text;
using System;
using System.Windows.Forms;

public class GameClientBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
        var commands = text.Split(new [] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
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
                _id = commandArguments[0];
                MessageBox.Show("Received ID: " + _id);
                break;
            }
        }
    }
}
