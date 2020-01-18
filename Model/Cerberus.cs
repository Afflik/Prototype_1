using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Game
{
    public class Cerberus : EnemySettings, IOnUpdate, IDamage
    {
        private BossUI _bossUi;
        
        [SerializeField]
        private float _FlameDmgInSec = 100;
        [SerializeField]
        private float _FlameDmgCd = 8;
        
        private float _distanceForPlayer;
        private float _flameTick;

        private bool _fightMode;
        private bool _isFlameHit;
        private bool _isFlameAttack;
        private bool _isJumpAttack;

        private GameObject _bossZone;

        private SphereCollider _attackCollider;

        private void Start()
        {
            _flameTick = 0;
            _attackSpeed = 1f;

            _defaultSpeed = _agent.speed;
            _rig.isKinematic = true;
            _agent.enabled = false;
            _bossUi = FindObjectOfType<BossUI>();
            _bossUi.BossName(gameObject.name);
            _bossZone = GameObject.FindGameObjectWithTag("BossZone");
            _bossUi.gameObject.SetActive(false);
            _bossZone.SetActive(false);
            _attackCollider = GetComponentInChildren<SphereCollider>();
            _attackCollider.enabled = false;
        }

        public void OnUpdate()
        {
            _distanceForPlayer = Vector3.Distance(_pl.transform.position, _trfm.position);
            _bossUi.GetDamage(_hp, _maxHp);

            if(!_fightMode && _distanceForPlayer < 25)
            {
                if (_hp < 0) return;
                _rig.isKinematic = false;
                _fightMode = true;
                FightMode();
                MoveSet(_defaultSpeed, 1);
            }

            if (!_fightMode) return;
            if (!_rig.isKinematic) LookAtPlayer();

            if (_hp < 0)
            {
                _fightMode = false;
                _agent.enabled = false;
                AttackSet(0, 0, 0);
                _anim.SetBool("dead", true);
                Invoke("FightMode", 2);
                return;
            }

            if (_anim.GetInteger("attack") == 3 || _isJumpAttack) return;

            if (_hp < _maxHp / 2)
            {
                _attackSpeed = 0.5f;
                if (!_isFlameAttack && !_isJumpAttack)
                {
                    _isFlameAttack = true;
                    AttackSet(0, 0, 3);
                    Invoke("FlameReset", _FlameDmgCd);
                }
            }
            _agent.SetDestination(_pl.transform.position);

            if (_distanceForPlayer < 12 && _distanceForPlayer > 9)
            {
                JumpAtack();
            }
            else if (_distanceForPlayer < 5 && !_rig.isKinematic)
            {
                _rig.velocity = Vector3.zero;
                if (Timer(_attackSpeed))
                {
                    SetTime(0);
                    AttackSet(0, 0, 1);
                }
                else AttackSet(0, 0, 0);
            }
            else
            {
               if(!_rig.isKinematic) AttackSet(_defaultSpeed, 1, 0);
            }
        }

        private void FightMode()
        {
            _agent.enabled = _fightMode;
            _bossUi.gameObject.SetActive(_fightMode);
            _bossZone.SetActive(_fightMode);
        }

        private void AttackSet(float speed, int move, int attack)
        {
            if (attack == 3) FlameStart();
            _anim.SetInteger("attack", attack);
            MoveSet(speed, move);
        }

        private void MoveSet(float speed, int move)
        {
            _agent.speed = speed;
            _anim.SetInteger("move", move);
        }

        private void JumpAtack()
        {
            _stunDmg = true;
            _isJumpAttack = true;
            AttackSet(0, 0, 2);
            _rig.velocity = _trfm.up * 7 + _trfm.forward * 16;
        }

        private void FlameStart()
        {
            _rig.isKinematic = true;
        }
        private void FlameEnd()
        {
            _rig.isKinematic = false;
            _anim.SetInteger("attack", 0);
        }
        private void FlameReset()
        {
            _isFlameAttack = false;
        }

        private void ResetVelocity()
        {
            _stunDmg = false;
            _isJumpAttack = false;
            _rig.velocity = Vector3.zero;
            _anim.SetInteger("attack", 0);
        }

        private void Dead()
        {
            _rig.isKinematic = true;
            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        public float Damage(float hp)
        {
           return hp -= _damage;
        }

        public bool StunDamage()
        {
            return _stunDmg;
        }
        public float SpecialDamage(float _hp)
        {

            _flameTick += Time.deltaTime;
            if(_flameTick > 1)
            {
                _flameTick = 0;
                return _hp -= _FlameDmgInSec;
            }
            return _hp;
        }
    }
}
