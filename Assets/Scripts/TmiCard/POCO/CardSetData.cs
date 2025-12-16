using System;
using System.Collections.Generic;

namespace TmiCard.POCO
{
    [Serializable]
    public abstract class CardSetData
    {
        public string name;
        public string author;
        public string description;
        public string version;
        public string apiVersion;
        public List<CardData> Cards;
    }
}