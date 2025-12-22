using TmiCard.CardCharacter;
using TMPro;
using UnityEngine;

namespace Managers.Card
{
    [RequireComponent(typeof(TMP_Text))]
    public class CardTitleManager: MonoBehaviour
    {
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }
        
        public void UpdateCharacterTitle(CardCharacter character)
        {
            text.text = character.Data.enName[0];
        }
    }
}