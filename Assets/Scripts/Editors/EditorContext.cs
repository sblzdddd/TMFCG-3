using R3;

namespace Editors
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