using System.Collections.Generic;
using System.IO;
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
        private readonly JsonSerializerSettings settings;
        
        private List<CharacterData> characters;
        public List<CharacterData> Characters => characters;

        public CharacterRepository()
        {
            settings = new JsonSerializerSettings();
            settings.Converters.Add(new Utils.Vector3Converter());
            settings.Formatting = Formatting.Indented;
            Load();
        }

        private void Load()
        {
            if (!File.Exists(configPath))
                InitDefault();

            var json = File.ReadAllText(configPath, Encoding.UTF8);
            characters = JsonConvert.DeserializeObject<List<CharacterData>>(json, settings);
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(characters, settings);
            File.WriteAllText(configPath, json);
        }

        private void InitDefault()
        {
            var config = ConfigFile.Replace(".json", "");
            var configText = Resources.Load<TextAsset>(config);
            File.WriteAllText(configPath, configText.text);
        }
    }
}
