namespace Game.Domain
{
    public enum HandType
    {
        Single = 0,
        // 22, 33, 44, 55
        Pair = 1,
        // 2233, 223344
        ConsecutivePairs = 2,
        // 23, 234, 2345, 23456
        Straight = 3,
        // 333, 4444, 55555
        NOfKind = 4,
        // JB & JR
        Rocket = 5,
    }
}