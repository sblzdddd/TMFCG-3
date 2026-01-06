using System.Collections.Generic;
using System.Linq;
using Game.Client.Managers.UI.Panels.Base;
using LitMotion;
using UnityEngine;

namespace Game.Client.Managers.UI.Panels.Panels
{
    public class StartPanel: BasePanel
    {
        [SerializeField, ChildObjectOnly, NotNull] public RectTransform title;
        [SerializeField, ChildObjectOnly, NotNull] public RectTransform startButton;
        [SerializeField, ChildObjectOnly, NotNull] public RectTransform customButton;
        [SerializeField, ChildObjectOnly, NotNull] public RectTransform settingsButton;
        [SerializeField, ChildObjectOnly, NotNull] public RectTransform exitButton;
        
        private List<RectTransform> startRects = new();

        private void Start()
        {
            FadePosition = -1;
            startRects = new List<RectTransform> {title, startButton, customButton, settingsButton, exitButton};
            Show();
        }

        public override void Hide(int direction = -1)
        {
            SetInteractability(false);
            if (direction == -1) return;
            var seq = LSequence.Create();
            foreach (var item in startRects.Select((button, i) => new { button, i }))
                seq.Insert(item.i * 0.02f, MoCo.FadeOut(direction, item.button));
            seq.Run();
        }

        public override void Show(int direction = -1)
        {
            SetInteractability(true);
            if (direction == -1) return;
            var seq = LSequence.Create();
            foreach (var item in startRects.Select((button, i) => new { button, i }))
                seq.Insert(item.i * 0.02f, MoCo.FadeIn(direction, item.button));
            seq.Run();
        }
    }
}
