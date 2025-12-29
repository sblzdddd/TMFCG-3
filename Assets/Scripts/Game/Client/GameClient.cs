using Game.Shared.Commands;
using Game.Shared.Snapshots;
using Net.Transport;
using UnityEngine;

namespace Game.Client
{
    public class GameClient
    {
        private GameStateSnapshot currentSnapshot;

        public void ApplySnapshot(GameStateSnapshot snapshot)
        {
            currentSnapshot = snapshot;
            Debug.Log($"[Client] ApplySnapshot: Turn={snapshot.Turn}");
        }

        public void PlayCard(string cardId, UtpClient net)
        {
            Debug.Log("[Client] Send PlayCardCmd");
            net.Send(new PlayCardCmd{ CardInstanceId = cardId });
        }
    }
}