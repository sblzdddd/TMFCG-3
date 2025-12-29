using System.Collections.Generic;
using System.Linq;
using Game.Client.Data.Repositories;
using Game.Shared.TmiCards.POCO;
using R3;

namespace Game.Client.Data.Providers
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