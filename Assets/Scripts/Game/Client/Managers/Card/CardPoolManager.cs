using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Client.Managers.Card
{
    public class CardPoolManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField, PrefabObjectOnly]
        private GameObject cardPrefab;

        private List<CardManager> cards;
        [Inject] private readonly IObjectResolver resolver;

        public CardManager Create()
        {
            // 由 VContainer 创建卡牌Prefab以正确依赖注入子组件
            var go = resolver.Instantiate(cardPrefab, transform);
            var card = go.GetComponent<CardManager>();
            if (card == null)
            {
                Debug.LogError($"Unable to find card manager in prefab {cardPrefab.name}!");
                return null;
            }
            card.Start();
            card.gameObject.SetActive(true);
            return card;
        }

        public void Dispose(CardManager card)
        {
            // 重置卡牌状态
            card.Dispose();
        }
    }
}