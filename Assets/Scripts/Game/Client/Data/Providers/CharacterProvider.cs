using System.Collections.Generic;
using System.Linq;
using Game.Client.Data.Repositories;
using Game.Shared.TmiCards.POCO;
using R3;

namespace Game.Client.Data.Providers
{
    public class CharacterProvider
    {
        private readonly CharacterRepository repository;
        private readonly Dictionary<string, CharacterData> charactersDict;

        public CharacterProvider(CharacterRepository characterRepository)
        {
            repository = characterRepository;
            charactersDict = repository.Characters.Distinct().ToDictionary(k => k.characterId);
        }
        
        public List<CharacterData> Characters => repository.Characters;

        public IEnumerable<CharacterData> ValidCharacters =>
            Characters.Where(characterData => characterData.variants.Count > 0);
        public List<CharacterData> ValidCharactersList =>
            Characters.Where(characterData => characterData.variants.Count > 0).ToList();

        public bool TryGet(string id, out CharacterData data) => charactersDict.TryGetValue(id, out data);

        public bool TryGetVariant(string id, int variantIndex, out string data)
        {
            data = null;
            if(!TryGet(id, out var characterData)) {return false;}
            data = characterData.variants[variantIndex];
            return true;
        }
    }
}