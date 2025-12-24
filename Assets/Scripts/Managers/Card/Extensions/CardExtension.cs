using UnityEngine;

namespace Managers.Card.Extensions
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