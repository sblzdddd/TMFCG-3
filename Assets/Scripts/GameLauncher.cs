using Game.Client;
using Game.Server;
using Game.Shared.Commands;
using Game.Shared.Snapshots;
using Net;
using Net.Transport;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    private UtpServer utpServer;
    private UtpClient utpClient;

    private GameServer gameServer;
    private GameClient gameClient;

    private void Start()
    {
        RegisterMessages();
    }

    private void RegisterMessages()
    {
        NetMessageRegistry.Register<PlayCardCmd>(1);
        NetMessageRegistry.Register<GameStateSnapshot>(100);
    }

    public void StartHost(ushort port=12637)
    {
        // --- Server ---
        gameServer = new GameServer();

        utpServer = new UtpServer();
        utpServer.Start(port);

        utpServer.Dispatcher.On<PlayCardCmd>(cmd =>
        {
            gameServer.Handle(cmd);

            var snapshot = gameServer.BuildSnapshot();
            utpServer.Broadcast(snapshot);
        });

        // --- Client ---
        StartClient("127.0.0.1", port);
    }

    public void StartClient(string host, ushort port=12637)
    {
        gameClient = new GameClient();

        utpClient = new UtpClient();
        utpClient.Connect(host, port);

        utpClient.Dispatcher.On<GameStateSnapshot>(snapshot =>
        {
            gameClient.ApplySnapshot(snapshot);
        });
    }

    private void Update()
    {
        utpServer?.Update();
        utpClient?.Update();
    }
}