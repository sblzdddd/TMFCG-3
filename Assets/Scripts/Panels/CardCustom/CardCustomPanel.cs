using Cysharp.Threading.Tasks;
using Interfaces.Card;
using Managers;
using Managers.Card;
using Managers.Card.Extensions;
using Panels.Base;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using R3;
using SimpleMystiaUI.Sliders;

namespace Panels.CardCustom
{
    [RequireComponent(typeof(RectTransform))]
    public class CardCustomPanel: BasePanel
    {
        [Header("UI")]
        [SerializeField, ChildObjectOnly, NotNull]
        public Button backButton;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterSize;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterX;
        [SerializeField, ChildObjectOnly, NotNull]
        private MSlider characterY;
        
        [Inject] private readonly CharacterGridView characterGridView;
        [Inject] private readonly CharacterEditor characterEditor;
        [Inject] private readonly CustomPreviewCard previewCard;
        
        private readonly CompositeDisposable disposables = new();

        public void Start()
        {
            FadePosition = 0;
            
            // 绑定滑条到EditTransform事件
            characterEditor.EditTransform
                .Subscribe(UpdateSliders)
                .AddTo(disposables);

            // 绑定滑条到Editor更新到滑条变化事件
            characterX.onValueChanged.AddListener(OnSliderChanged);
            characterY.onValueChanged.AddListener(OnSliderChanged);
            characterSize.onValueChanged.AddListener(OnSliderChanged);
            
            backButton.onClick.AddListener(characterEditor.SaveConfig);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }

        private void UpdateSliders(Vector3 t)
        {
            characterX.SetValueWithoutNotify(t.x);
            characterY.SetValueWithoutNotify(t.y);
            characterSize.SetValueWithoutNotify(t.z);
        }
        
        private void OnSliderChanged(float _)
        {
            characterEditor.UpdateTransform(new Vector3(
                characterX.value, 
                characterY.value, 
                characterSize.value
            ));
        }

        public override void Show(int direction = -1)
        {
            characterGridView.RefreshGrid().Forget();
            previewCard.Show();
            base.Show(direction);
        }
        
        public override void Hide(int direction = -1)
        {
            previewCard.Hide();
            base.Hide(direction);
        }
    }
}