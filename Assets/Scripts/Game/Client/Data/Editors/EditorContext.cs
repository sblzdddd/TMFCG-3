using R3;

namespace Game.Client.Data.Editors
{
    public enum PreviewContext
    {
        CardEditor,
        CharacterEditor
    }
    
    public class EditorContext
    {
        public readonly ReactiveProperty<PreviewContext> CurrentContext =
            new(PreviewContext.CardEditor);
    }
}