using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using TmiCard.POCO;

namespace Managers
{
    public class CharacterConfigService: MonoBehaviour
    {
        [ScrollableItems]
        public List<CharacterData> characters = new();

        private const string ConfigFile = "Characters.json";
        private string configPath;

        private void Awake()
        {
            configPath = Path.Combine(Application.persistentDataPath, ConfigFile);
            if (!File.Exists(configPath))
            {
                InitCharacterConfig();    
            }
            LoadCharacterConfig();
        }

        private void LoadCharacterConfig()
        {
            var json = File.ReadAllText(configPath, Encoding.UTF8);
            characters = JsonConvert.DeserializeObject<List<CharacterData>>(json);
        }

        private void InitCharacterConfig()
        {
            Debug.Log("Characters config file not found, copying from Resources folder.");
            var config = ConfigFile.Replace(".json", "");
            Debug.Log(config);
            var configText = Resources.Load<TextAsset>(config);

            if (configText == null)
            {
                Debug.LogError("Characters config file corrupted");
                return;
            }

            try
            {
                File.WriteAllText(configPath, configText.text);
                Debug.Log($"Default config written to {configPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write Characters config file. {e.Message}");
            }
        }
    }
}