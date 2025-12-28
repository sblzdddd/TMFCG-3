using System;
using System.Collections.Generic;
using UnityEngine;

namespace TmiCard.POCO
{
    [Serializable]
    public class CharacterData
    {
        public string characterId = "FALLBACK";
        public string name = "Fallback";
        [IgnoreParent]
        public List<string> enName = new();
        public string jpName = "Fallback";
        [IgnoreParent]
        public List<string> nickname =  new();
        public string description = "Fallback";
        public string first = "Fallback";
        [ReorderableList]
        public Vector3 transform = new(0,0,1);
        [ReorderableList]
        public List<string> variants = new();
    }
}