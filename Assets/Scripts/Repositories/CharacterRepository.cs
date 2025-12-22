using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TmiCard.POCO;
using UnityEngine;

namespace Repositories
{
    public class CharacterRepository
    {
        private const string ConfigFile = "Characters.json";
        private readonly string configPath = Path.Combine(Application.persistentDataPath, ConfigFile);
        
        private List<CharacterData> characters;
        public IReadOnlyList<CharacterData> Characters => characters;
        private readonly Dictionary<string, CharacterData> charactersDict;

        public CharacterRepository()
        {
            Load();
            charactersDict = characters.Distinct().ToDictionary(k => k.characterId);
        }

        private void Load()
        {
            if (!File.Exists(configPath))
                InitDefault();

            var json = File.ReadAllText(configPath, Encoding.UTF8);
            characters = JsonConvert.DeserializeObject<List<CharacterData>>(json);
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(characters, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }
        
        public CharacterData Get(string id)
        {
            if (charactersDict.TryGetValue(id, out var character)) return character;
            Debug.LogWarning($"Requested CharacterId={id} not found!");
            return null;
        } 

        private void InitDefault()
        {
            var config = ConfigFile.Replace(".json", "");
            var configText = Resources.Load<TextAsset>(config);
            File.WriteAllText(configPath, configText.text);
        }
    }
}
