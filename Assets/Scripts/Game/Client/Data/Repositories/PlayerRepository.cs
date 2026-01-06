using System.IO;
using System.Text;
using Game.Shared.TmiCards.POCO;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace Game.Client.Data.Repositories
{
    public class PlayerRepository
    {
        private const string ConfigFile = "Player.json";
        private readonly string configPath = Path.Combine(Application.persistentDataPath, ConfigFile);
        private readonly JsonSerializerSettings settings;
    
        public readonly ReactiveProperty<PlayerInfoData> PlayerInfo;

        public PlayerRepository()
        {
            PlayerInfo = new ReactiveProperty<PlayerInfoData>(Load());
        }

        private PlayerInfoData Load()
        {
            if (!File.Exists(configPath)) return null;

            var json = File.ReadAllText(configPath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<PlayerInfoData>(json);
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(PlayerInfo.CurrentValue, settings);
            File.WriteAllText(configPath, json);
        }
    }
}