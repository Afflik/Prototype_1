using UnityEngine;
using System;

namespace Game
{
    public class PlayerStats : MainAbs
    {
        [Serializable]
        public struct Stats
        {
            public float expScale;
            public float expForLevel;
            public float hp;
            public float hpScaleLavel;
            public float energy;
            public float energyPerSec;
            public float speed;
            public float critDamage;
            public float critChance;
            public float dmgScalePhysical;
            public float dmgScaleMagic;
        }
        public Stats Player;
    }

}
