using UnityEngine;

namespace Game.Client.Managers.Card.Extensions
{
    public abstract class CardExtension : MonoBehaviour
    {
        protected CardManager CardManager;

        public virtual void Initialize(CardManager cardManager)
        {
            CardManager = cardManager;
        }

        public virtual void OnActivate() {}
        public virtual void OnDeactivate() {}
    }
}