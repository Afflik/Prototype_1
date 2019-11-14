using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class PlayerSettings : MainAbs
    {
        private SpellManager _spellBook;  // коллекция способностей
        protected SpellUI _spellUi;
        protected PlayerUI _playerUi;

        [SerializeField]
        protected List<GameObject> _effects = new List<GameObject>(); // лист различных вспомогательных эффектов

        [SerializeField]
        protected float _hp = 100f; // скорость персонажа
        [SerializeField]
        protected float _speedRestHp = 2f; // скорость персонажа
        [SerializeField]
        protected float _energy = 100f; // скорость персонажа
        [SerializeField]
        protected float _speedRestEn = 4f; // скорость персонажа
        [SerializeField]
        protected float _speed = 4f; // скорость персонажа
		
        protected float _basicSpeed; // сохранение базовой скорости персонажа
        protected float _defaultAttackCost; // стоимость тяжелой атаки

        protected int _move = 0; // состояние передвижения
        protected int _attackType = 0; // состояние атаки
        protected int _comboLeftClick = 0; // состояние серии легких атак
        protected int _comboRightClick = 0; // состояние серии тяжелых атак
        protected int _spellLast = -1; // последний используемый каст
        protected int _spellNow = -1; // текущий каст

        protected bool _isRoll; // состояние кувырка
        protected bool _isBuffOn; // состояние кувырка
        protected bool _isFreezeRot; // заморока поворота
        protected bool _checkGroundDist; // проверка дистанции до земли
        
        protected Vector3 _distForGround; // дистанция до земли
        protected Vector3 _defaultScale; // базовый размер персонажа
        protected Vector3 _scaleUpStep; // шаг изменения размера персонажа
        protected Vector3 _buffScale; // измененный размер персонажа

        public float Speed { get => _speed; set => _speed = value; }
        
        public SpellManager.Spell[] SpellBook { get => _spellBook.Spells; set => _spellBook.Spells = value; }

        public new void Awake()
        {
            base.Awake();

            _spellBook = FindObjectOfType<SpellManager>(); 
            _spellUi = FindObjectOfType<SpellUI>();
            _playerUi = FindObjectOfType<PlayerUI>();

            _playerUi.SpRestEnergy = _speedRestEn;

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
                SpellBook[i].spell.SetActive(false);
                SpellBook[i].id = i;
            }
        }
    }
}
