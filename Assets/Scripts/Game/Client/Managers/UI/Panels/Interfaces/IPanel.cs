using Cysharp.Threading.Tasks;

namespace Game.Client.Managers.UI.Panels.Interfaces
{
    public interface IPanel
    {
        void OnInitialize();
        UniTask OnClose();
        void OnOpen();
        void OnFocus();
        void OnLostFocus();
    }
}