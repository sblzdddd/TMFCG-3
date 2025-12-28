using System.Collections.Generic;
using System.Linq;
using R3;
using Repositories;
using TmiCard.Card;
using TmiCard.POCO;
using UnityEngine;

namespace Providers
{
    public class CharacterProvider
    {
        public readonly ReactiveProperty<string> CurrentDeckName = new();
        private readonly CharacterRepository repository;
        private readonly Dictionary<string, CharacterData> charactersDict;

        public CharacterProvider(CharacterRepository characterRepository)
        {
            repository = characterRepository;
            charactersDict = repository.Characters.Distinct().ToDictionary(k => k.characterId);
        }
        
        public List<CharacterData> Characters => repository.Characters;

        public bool TryGet(string id, out CharacterData data) => charactersDict.TryGetValue(id, out data);
    }
}