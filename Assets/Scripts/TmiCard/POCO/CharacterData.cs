using System;
using System.Collections.Generic;
using UnityEngine;

namespace TmiCard.POCO
{
    [Serializable]
    public class CharacterVariant
    {
        public string image;
        [ReorderableList]
        public List<float> transform;
    }
    
    [Serializable]
    public class CharacterData
    {
        public string characterId = "FALLBACK";
        public string name;
        [IgnoreParent]
        public List<string> enName;
        public string jpName;
        [IgnoreParent]
        public List<string> nickname;
        public string description;
        public string first;
        [ReorderableList]
        public List<CharacterVariant> variants;
    }
}