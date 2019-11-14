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
        public struct Spell // анкетка добавления способности
        {
            public GameObject spell;
            public bool isBuff;
            public bool isAttack;
            public string name;
            public int id;
            public float cd;
            public float time;
            public float cost;
            public bool status;
        }
        public Spell[] Spells;
    }
    
}
