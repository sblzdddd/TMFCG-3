using MessagePack;

namespace Game.Shared.Snapshots
{
    [MessagePackObject]
    public class GameStateSnapshot
    {
        [Key(0)] public int Turn;
        [Key(1)] public string Info;
    }
}