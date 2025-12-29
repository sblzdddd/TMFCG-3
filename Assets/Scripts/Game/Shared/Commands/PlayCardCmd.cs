using MessagePack;

namespace Game.Shared.Commands
{
    [MessagePackObject]
    public class PlayCardCmd
    {
        [Key(0)]
        public string CardInstanceId;
    }
}