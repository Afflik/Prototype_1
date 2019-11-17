using UnityEngine;
using System;

namespace Game
{ 
    public class SpellManager: MainAbs
    {
        [Serializable]
        public struct Spell // анкетка добавления способности
        {
            public GameObject spell;
            public bool isBuff;
            public bool isAttack;
            public string name;
            public int id;
            public float dmg;
            public float cd;
            public float time;
            public float cost;
            public bool status;

            public int feature1;
            public int feature2;
            public int feature3;
        }
        public Spell[] Spells;
    }
    
}
