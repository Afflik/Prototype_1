using UnityEngine;

namespace Game
{
    public class EnemyMelee : EnemyAI, IOnUpdate, IDamage
    {
        private void Start()
        {
            RngWanderPos();
            Patrol();
        }
        public void OnUpdate()
        {
            if (_isStunned)
            {
                _agent.speed = 0;
                return;
            }
            if (_isHit) return;
            if (_dead) return;

            _agent.speed = 1;
            if (Vector3.Distance(_trfm.position, _pl.transform.position) < 12)
            {
                LookAtPlayer();
                if (Vector3.Distance(_trfm.position, _pl.transform.position) < 2)
                {
                    if (!_isAttack) Attack(true);
                    return;
                }
                else
                {
                   Attack(false);
                }
                _agent.speed = 3;
                _anim.SetBool("charge", true);
                _agent.SetDestination(_pl.transform.position);
            }
            else
            {
                _anim.SetBool("charge", false);
                RngWanderPos();
                Patrol();
            }
        }
        private void Attack(bool attack)
        {
            _isAttack = attack;
            _anim.SetBool("move", false);
            _anim.SetBool("charge", false);
            if (!_isAttack)
            {
                _anim.SetInteger("attack", 0);
            }
            else
            {
                _anim.SetInteger("attack", Random.Range(1, 4));
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

        public float SpecialDamage(float hp)
        {
            return 0;
        }
    }
}
