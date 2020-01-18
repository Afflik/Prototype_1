using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class PlayerSettings : MainAbs, IPotion
    {
        private PlayerStats.Stats _stats;  // коллекция способностей
        private SpellManager _spellBook;  // коллекция способностей
        private ItemData _items;  // база итемов
        protected ActionBarUI _barUi;
        protected PlayerUI _playerUi;
        protected ExpBarUI _expBar;

        [SerializeField]
        protected List<GameObject> _effects = new List<GameObject>(); // лист различных вспомогательных эффектов


        protected float _hp; // хп персонажа
        protected float _maxHp;
        protected float _energy; // количество энергии персонажа
        protected float _maxEnergy;
        protected float _experience;
        protected float _expForLevel;
        protected float _critChance;
        protected float _speedRestEn; // скорость восстановления энергии
        protected float _speed; // скорость персонажа
        protected float _speedRestEnDefault;
        protected float _basicSpeed; // сохранение базовой скорости персонажа
        protected float _defaultAttackCost; // стоимость тяжелой атаки

        protected int _level;
        protected int _points;
        protected int _activePotion;
        protected int _move = 0; // состояние передвижения
        protected int _attackType = 0; // состояние атаки
        protected int _comboLeftClick = 0; // состояние серии легких атак
        protected int _comboRightClick = 0; // состояние серии тяжелых атак
        protected int _spellLast = -1; // последний используемый каст
        protected int _spellNow = -1; // текущий каст

        protected bool _isDeath;
        protected bool _isStunned;
        protected bool _isRoll; // состояние кувырка
        protected bool _lockLook; // состояние слежения за курсором
        protected bool _isBuffOn; // состояние кувырка
        protected bool _isFreezeRot; // заморока поворота
        protected bool _checkGroundDist; // проверка дистанции до земли

        protected bool _isHealPot;
        protected bool _isEnergyPot;
        protected bool _isPowerPot;

        protected Vector3 _distForGround; // дистанция до земли
        protected Vector3 _defaultScale; // базовый размер персонажа
        protected Vector3 _scaleUpStep; // шаг изменения размера персонажа
        protected Vector3 _buffScale; // измененный размер персонажа

        protected int[] potion = new int[3];

        public float Speed { get => _speed; set => _speed = value; }
        public SpellManager.Spell[] SpellBook { get => _spellBook.Spells; set => _spellBook.Spells = value; }
        public ItemData.Item[] Item { get => _items.Items; set => _items.Items = value; }
        public int[] Potion { get => potion; set => potion = value; }
        public int ActivePotion { get => _activePotion; set => _activePotion = value; }
        public int AttackType { get => _attackType; set => _attackType = value; }
        public bool IsStunned { get => _isStunned; set => _isStunned = value; }
        public float Hp { get => _hp; set => _hp = value; }
        public float MaxHp { get => _maxHp; set => _maxHp = value; }
        public float MaxEnergy { get => _maxEnergy; set => _maxEnergy = value; }
        public float Crit { get => _critChance; set => _critChance = value; }
        public bool IsDeath { get => _isDeath; set => _isDeath = value; }
        public PlayerStats.Stats Stats { get => _stats; set => _stats = value; }
        public float Energy { get => _energy; set => _energy = value; }

        protected SpellManager.Spell[] SpellBookDefault;

        public new void Awake()
        {
            base.Awake();
            _items = FindObjectOfType<ItemData>();
            _stats = FindObjectOfType<PlayerStats>().Player;
            _spellBook = FindObjectOfType<SpellManager>();
            _barUi = FindObjectOfType<ActionBarUI>();
            _playerUi = FindObjectOfType<PlayerUI>();
            _expBar = FindObjectOfType<ExpBarUI>();

            LoadStats();

            _level = 1;
            _hp = _maxHp;
            _energy = _maxEnergy;
            _playerUi.SpRestEnergy = _speedRestEn;
            _speedRestEnDefault = _speedRestEn;

            _buffScale = new Vector3(1, 1, 1);
            _defaultScale = new Vector3(0.8f, 0.8f, 0.8f);
            _scaleUpStep = new Vector3(0.05f, 0.05f, 0.05f);
            _distForGround = transform.TransformDirection(Vector3.down);
            _basicSpeed = _speed;

            for (int i = 0; i < SpellBook.Length; i++)
            {
                if (SpellBook[i].isAttack)
                {
                    _defaultAttackCost = SpellBook[i].cost;
                }
                else SpellBook[i].spell.SetActive(false);
                SpellBook[i].id = i;
            }
            SpellBookDefault = new List<SpellManager.Spell>(SpellBook).ToArray();
        }

        private void LoadStats()
        {
            _maxHp = _stats.hp;
            _maxEnergy = _stats.energy;
            _expForLevel = _stats.expForLevel;
            _speedRestEn = _stats.energyPerSec;
            _speed = _stats.speed;
            _critChance = _stats.critChance;

            for (int i = 0; i < SpellBook.Length; i++)
            {
                if(SpellBook[i].isAttack)
                {
                    if (_stats.dmgScalePhysical > 0) SpellBook[i].dmg += SpellBook[i].dmg * _stats.dmgScalePhysical / 100;
                }
                if (_stats.dmgScaleMagic > 0) SpellBook[i].dmg += SpellBook[i].dmg * _stats.dmgScaleMagic / 100;
            }
        }

        private void LevelUp()
        {
            _level++;
            _points++;
            _maxHp += _maxHp * _stats.hpScaleLavel / 100;
            _hp = _maxHp;
            _playerUi.Health(_hp, _maxHp);
        }
        public void GetExp(float exp)
        {
            _experience += exp;
            if (_expBar.GetExp(exp)) LevelUp();
        }

        public IEnumerator SpellBookDoDefault(int id, float time)
        {
            yield return new WaitForSeconds(time);
            if (id != 5)
            {
                for (int i = 0; i < SpellBook.Length; i++)
                {
                    if (id == 1) SpellBook[i].cd = SpellBookDefault[i].cd;
                    else if (id == 2) SpellBook[i].dmg = SpellBookDefault[i].dmg;
                }
            }
            if (id == 1)
            {
                _isEnergyPot = false;
            }
            else if (id == 2)
            {
                _isPowerPot = false;
            }
            else if (id == 5)
            {
                SpellBook[SpellBook.Length - 2].dmg = SpellBookDefault[SpellBook.Length - 2].dmg;
            }
            StopCoroutine("SpellBookDoDefault");

        }

        public void TakePotion(int id) // подобрали банку
        {
            Potion[id]++;
            _barUi.AddPotion(id);
        }

        public void RemotePotion(int id) // использовали банку
        {
            Potion[id]--;
            _barUi.RemotePotion(id);
        }

        public void HealthPotion(int id, float heal) // хил банка
        {
            _isHealPot = true;
            Hp += _maxHp * heal / 100;
            if (Hp > _maxHp) Hp = _maxHp;
            _playerUi.Health(_hp, _maxHp);
            RemotePotion(id);
        }

        public void EnergyPotion(int id, float enBoost, float cdBoost, float time) // банка с энергией
        {
            if (!_isEnergyPot) // если еще нету бафа от текущей банки, добавляем бафф
            {
                _barUi.AddBuff(id, time); // добавляем иконку баффа
                for (int i = 0; i < SpellBook.Length; i++)
                {
                    SpellBook[i].cd -= (SpellBook[i].cd * cdBoost / 100);
                };
            }
            _isEnergyPot = true;

            RemotePotion(id); // выкидываем использованную банку
            enBoost = _speedRestEn + (_speedRestEnDefault * enBoost / 100); // задаем новую скорость восстановления энергии
            
            _playerUi.EnergyPotion(enBoost, time);  // передаем интерфейсу новые данные
            StartCoroutine(SpellBookDoDefault(id, time)); // возврат измененных данных после окончания баффа
        }

        public void PowerPotion(int id, float dmgUp, float time)
        {
            if (!_isPowerPot) // если еще нету бафа от текущей банки, добавляем бафф
            {
                _barUi.AddBuff(id, time); // добавляем иконку баффа
                for (int i = 0; i < SpellBook.Length; i++)
                {
                    SpellBook[i].dmg += (SpellBook[i].dmg * dmgUp / 100);
                }
            }
            _isPowerPot = true;
            RemotePotion(id); // выкидываем использованную банку
            StartCoroutine(SpellBookDoDefault(id, time)); // возврат измененных данных после окончания баффа
        }
    }
}
