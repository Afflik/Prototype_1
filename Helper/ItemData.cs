using System;
using UnityEngine;

namespace Game
{
    public class ItemData : MainAbs
    {
        [Serializable]
        public struct Item // анкетка добавления способности
        {
            public GameObject item;
            public bool canDrop;
            public string name;
            public int id;
            public int dropChance;

            public int feature1;
            public int feature2;
            public int feature3;

            public bool status;
        }
        public Item[] Items;
    }
}

