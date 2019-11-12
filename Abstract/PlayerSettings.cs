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
        protected List<GameObject> _effects = new List<GameObject>();

        [SerializeField]
        protected float _speed = 4f;
		
        protected float _basicSpeed;
        protected float _buffTime;

        protected int _move = 0;
        protected int _attackType = 0;
        protected int _comboLeftClick = 0;
        protected int _comboRightClick = 0;
        
        protected bool _isRoll;
        protected bool _isBuffOn;
        protected bool _isMove = true;
        protected bool _isFreezeRot;
        protected bool _checkGroundDist;

        protected GameObject _spellLast;
        protected GameObject _spellNow;

        protected Vector3 _distForGround;
        protected Vector3 _defaultScale;
        protected Vector3 _scaleUpStep;
        protected Vector3 _buffScale;

        public float Speed { get => _speed; set => _speed = value; }

        public GameObject HurricaneSpell { get => _hurricaneSpell; set => _hurricaneSpell = value; }
        public GameObject FrostJumpSpell { get => _frostJumpSpell; set => _frostJumpSpell = value; }
        public GameObject LightingStrikeSpell { get => _lightingStrikeSpell; set => _lightingStrikeSpell = value; }
        public GameObject RampageBuff { get => _rampageBuff; set => _rampageBuff = value; }

        protected List<GameObject> _spellBook = new List<GameObject>();

        public void Awake()
        {
            base.Awake();
            _buffScale = new Vector3(1, 1, 1);
            _defaultScale = new Vector3(0.8f, 0.8f, 0.8f);
            _scaleUpStep = new Vector3(0.05f, 0.05f, 0.05f);
            _distForGround = transform.TransformDirection(Vector3.down);
            _basicSpeed = _speed;
            _isFreezeRot = false;
            
            _spellBook.Add(null);
            _spellBook.Add(_hurricaneSpell);
            _spellBook.Add(_frostJumpSpell);
            _spellBook.Add(_lightingStrikeSpell);
            _spellBook.Add(_rampageBuff);
        }
    }
}
