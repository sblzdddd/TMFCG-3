using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Client.Data.Providers
{
    public class RandomPlayerInfoProvider
    {
        private readonly CharacterProvider characterProvider;
        private static readonly Random Random = new();

        private static T PickRandomItem<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            int index = Random.Next(list.Count);
            return list[index];
        }
        
        public RandomPlayerInfoProvider(CharacterProvider characterProvider)
        {
            this.characterProvider = characterProvider;
        }

        public string GetRandomPlayerName()
        {
            var prefixes = new List<string>();
            var suffixes = new List<string>();
            foreach (var characterData in characterProvider.Characters)
            {
                if (characterData.name.Length < 5) suffixes.Add(characterData.name);
                suffixes.AddRange(characterData.nickname);
                if (characterData.variants.Count > 0 && characterData.variants[0].Split(" ")[0].Length < 5)
                    suffixes.Add(characterData.variants[0]);
                
                var parts = characterData.description.Split("的");
                if (parts.Length > 0 && parts[0].Length <= 6)
                    prefixes.Add(parts[0] + "的");
                parts = characterData.description.Split("之");
                if (parts.Length > 0 && parts[0].Length <= 6)
                    prefixes.Add(parts[0] + "之");
            }

            return $"{PickRandomItem(prefixes)}{PickRandomItem(suffixes)}";
        }
        public (string, int) GetRandomCharacter()
        {
            var character = PickRandomItem(characterProvider.ValidCharactersList);

            return (character.characterId, Random.Next(character.variants.Count));
        }
        
    }
}