using Game.Shared.Commands;
using Game.Shared.Snapshots;
using UnityEngine;

namespace Game.Server
{

    public class GameServer
    {
        private const int Turn = 1;

        public void Handle<T>(T cmd)
        {
            switch (cmd)
            {
                case PlayCardCmd playCardCmd:
                    HandlePlayCard(playCardCmd);
                    break;
            }
            
        }

        private void HandlePlayCard(PlayCardCmd cmd)
        {
            Debug.Log($"[Server] Handle PlayCardCmd: {cmd.CardInstanceId}");

            // TODO:
            // Validate
            // State Update
            // Turn Update
        }

        public GameStateSnapshot BuildSnapshot()
        {
            Debug.Log("[Server] BuildSnapshot");

            return new GameStateSnapshot
            {
                Turn = Turn,
                Info = "Hello from server"
            };
        }
    }

}