using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Game
{
    public class SpellManager : MonoBehaviour
    {
        [Serializable]
        public struct Spell // анкетка добавления способности в инспекторе
        {
            public GameObject spell;
            public string name;
            public int cd;
            public int time;
            public int cost;
        }
        public Spell[] Spells;
    }
    
}
