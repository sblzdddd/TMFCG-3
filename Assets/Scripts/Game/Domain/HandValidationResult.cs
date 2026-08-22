namespace Game.Domain
{

    public record HandValidationResult(bool Valid, HandError Type, string Msg);
}