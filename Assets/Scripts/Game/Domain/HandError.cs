namespace Game.Domain
{
    public enum HandError
    {
        NoError = 0,
        // the hand played does not have a valid type
        Incorrect = 1,
        // Cannot play the type due to previous type constraints
        Inconsistent = 2,
        // the type is correct, but lower value
        LowerValue = 3,
    }
}