using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class PlayerSettings : MainAbs
    {
        [SerializeField]
        protected GameObject _hurricaneSpell;
        [SerializeField]
        protected GameObject _frostJumpSpell;
        [SerializeField]
        protected GameObject _lightingStrikeSpell;
        [SerializeField]
        protected GameObject _rampageBuff;
        [SerializeField]
        protected List<GameObject> _effects = new List<GameObject>(); // лист различных вспомогательных эффектов

        [SerializeField]
        protected float _speed = 4f; // скорость персонажа
		
        protected float _basicSpeed; // сохранение базовой скорости персонажа
        protected float _buffRampageTime;  // продолжительность баффа

        protected int _move = 0; // состояние передвижения
        protected int _attackType = 0; // состояние атаки
        protected int _comboLeftClick = 0; // состояние серии легких атак
        protected int _comboRightClick = 0; // состояние серии тяжелых атак

        protected bool _isRoll; // состояние кувырка
        protected bool _isBuffOn; // состояние кувырка
        protected bool _isFreezeRot; // заморока поворота
        protected bool _checkGroundDist; // проверка дистанции до земли

        protected GameObject _spellLast; // последний используемый каст
        protected GameObject _spellNow; // текущий каст

        protected Vector3 _distForGround; // дистанция до земли
        protected Vector3 _defaultScale; // базовый размер персонажа
        protected Vector3 _scaleUpStep; // шаг изменения размера персонажа
        protected Vector3 _buffScale; // измененный размер персонажа

        public float Speed { get => _speed; set => _speed = value; }

        public GameObject HurricaneSpell { get => _hurricaneSpell; set => _hurricaneSpell = value; }
        public GameObject FrostJumpSpell { get => _frostJumpSpell; set => _frostJumpSpell = value; }
        public GameObject LightingStrikeSpell { get => _lightingStrikeSpell; set => _lightingStrikeSpell = value; }
        public GameObject RampageBuff { get => _rampageBuff; set => _rampageBuff = value; }

        protected List<GameObject> _spellBook = new List<GameObject>(); // коллекция способностей

        public void Awake()
        {
            base.Awake();
            _buffScale = new Vector3(1, 1, 1);
            _defaultScale = new Vector3(0.8f, 0.8f, 0.8f);
            _scaleUpStep = new Vector3(0.05f, 0.05f, 0.05f);
            _distForGround = transform.TransformDirection(Vector3.down);
            _basicSpeed = _speed;
            
            _spellBook.Add(null);
            _spellBook.Add(_hurricaneSpell);
            _spellBook.Add(_frostJumpSpell);
            _spellBook.Add(_lightingStrikeSpell);
            _spellBook.Add(_rampageBuff);
        }
    }
}
