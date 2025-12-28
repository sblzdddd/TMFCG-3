using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TmiCard.POCO;
using UnityEngine;

namespace Repositories
{
    public class DeckRepository
    {
        private const string ConfigFile = "Decks.json";
        private readonly string configPath = Path.Combine(Application.persistentDataPath, ConfigFile);
        private readonly JsonSerializerSettings settings;
        
        private List<DeckData> decks;
        public IReadOnlyList<DeckData> Decks => decks;
        private readonly Dictionary<string, DeckData> decksDict;

        public DeckRepository()
        {
            settings = new JsonSerializerSettings();
            settings.Converters.Add(new Utils.Vector3Converter());
            settings.Formatting = Formatting.Indented;
            Load();
            decksDict = decks.Distinct().ToDictionary(k => k.name);
        }

        private void Load()
        {
            if (!File.Exists(configPath))
                InitDefault();

            var json = File.ReadAllText(configPath, Encoding.UTF8);
            decks = JsonConvert.DeserializeObject<List<DeckData>>(json, settings);
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(decks, settings);
            File.WriteAllText(configPath, json);
        }

        private void InitDefault()
        {
            var defaultConf = Resources.Load<TextAsset>("default_decks");
            File.WriteAllText(configPath, defaultConf.text);
        }
    }
}