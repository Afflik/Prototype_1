using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public abstract class PlayerSettings : MainAbs, IPotion
    {
        private SpellManager _spellBook;  // коллекция способностей
        private ItemData _items;  // база итемов
        protected ActionBarUI _barUi;
        protected PlayerUI _playerUi;
        protected Weapon _hammer;    // оружие

        [SerializeField]
        protected List<GameObject> _effects = new List<GameObject>(); // лист различных вспомогательных эффектов

        [SerializeField]
        protected float _hp = 1000f; // скорость персонажа
        [SerializeField]
        protected float _speedRestHp = 2f; // скорость персонажа
        [SerializeField]
        protected float _energy = 100f; // скорость персонажа
        [SerializeField]
        protected float _speedRestEn = 4f; // скорость персонажа
        [SerializeField]
        protected float _speed = 4f; // скорость персонажа

        protected float _maxHp;
        protected float _speedRestEnDefault;
        protected float _basicSpeed; // сохранение базовой скорости персонажа
        protected float _defaultAttackCost; // стоимость тяжелой атаки

        protected int _activePotion;
        protected int _move = 0; // состояние передвижения
        protected int _attackType = 0; // состояние атаки
        protected int _comboLeftClick = 0; // состояние серии легких атак
        protected int _comboRightClick = 0; // состояние серии тяжелых атак
        protected int _spellLast = -1; // последний используемый каст
        protected int _spellNow = -1; // текущий каст

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

        private int[] potion = new int[3];

        public float Speed { get => _speed; set => _speed = value; }
        public SpellManager.Spell[] SpellBook { get => _spellBook.Spells; set => _spellBook.Spells = value; }
        public ItemData.Item[] Item { get => _items.Items; set => _items.Items = value; }
        public int[] Potion { get => potion; set => potion = value; }
        public int ActivePotion { get => _activePotion; set => _activePotion = value; }
        public int AttackType { get => _attackType; set => _attackType = value; }

        public SpellManager.Spell[] SpellBookDefault;

        public new void Awake()
        {
            base.Awake();

            _maxHp = _hp;
            _items = FindObjectOfType<ItemData>();
            _spellBook = FindObjectOfType<SpellManager>();
            _barUi = FindObjectOfType<ActionBarUI>();
            _playerUi = FindObjectOfType<PlayerUI>();

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

            _hammer = transform.GetComponentInChildren<Weapon>();
        }

        public IEnumerator SpellBookDoDefault(int id, float time)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < SpellBook.Length; i++)
            {
                if (id == 1) SpellBook[i].cd = SpellBookDefault[i].cd;
                else if (id == 2) SpellBook[i].dmg = SpellBookDefault[i].dmg;
            }
            if (id == 1)
            {
                _isEnergyPot = false;
            }
            else if (id == 2)
            {
                _isPowerPot = false;
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
            _hp += (_maxHp * heal / 100);
            if (_hp > 100) _hp = 100;
            _playerUi.HealthPotion(_hp);
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
