using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public abstract class EnemySettings : MainAbs
    {
        [SerializeField]
        protected float _maxHp;
        [SerializeField]
        protected float _damage;
        [SerializeField]
        protected float _experience;

        protected float _hp;
        protected float _defaultSpeed;
        protected float _attackSpeed;
        
        protected PlayerModel _pl;
        protected NavMeshAgent _agent;

        protected SpellManager.Spell[] _playerSpells;
        protected ItemData _itemList;

        protected Collider _col;

        protected bool _stunDmg;
        protected bool _isStunned;
        protected bool _isDmgDone;
        protected bool _isAttack;
        protected bool _dead;
        protected bool _isHit;

        protected ItemData.Item[] DropList { get => _itemList.Items; set => _itemList.Items = value; }

        public virtual new void Awake()
        {
            base.Awake();
            _hp = _maxHp;
            _agent = GetComponent<NavMeshAgent>();
            _pl = FindObjectOfType<PlayerModel>();
            _col = gameObject.GetComponent<Collider>();
            _itemList = GameObject.FindObjectOfType<ItemData>();
            _playerSpells = FindObjectOfType<SpellManager>().Spells;
            _defaultSpeed = _agent.speed;

            if (!_trfm.CompareTag("Boss"))
            {
                foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>()) // отключение физики для рэгдол элементов
                {
                    r.isKinematic = true;
                }
            }
        }

        public virtual void LookAtPlayer()
        {
            Quaternion lookAt = Quaternion.LookRotation(_pl.transform.position - _trfm.position);
            _trfm.rotation = Quaternion.Slerp(_trfm.rotation, lookAt, Time.deltaTime * 5);
        }

        public virtual void Stunned()
        {
            if (_isStunned) _isStunned = false;
            else
            {
                _isStunned = true;
                Invoke(nameof(Stunned), 2);
            }

            _anim.SetBool("stunned", _isStunned);
        }

        public virtual void DropCheck() // дроп баночек
        {
            int min = 0;
            int max = 0;
            int drop = Random.Range(0, 101);
            for (int i = 0; i < DropList.Length; i++)
            {
                if (i == 0)
                {
                    max += DropList[i].dropChance;
                }
                else
                {
                    min += DropList[i - 1].dropChance;
                    max += DropList[i].dropChance;
                }

                if (drop >= min && drop <= max)
                {
                    Drop(i, DropList[i].item);
                    break;
                }
            }
        }

        public virtual void Drop(int id, GameObject item) // если шанс сработал, с врага падает банка
        {
            string itemName = item.name;
            var drop = Instantiate(item, _trfm.position, Quaternion.identity);
            drop.GetComponent<Potion>().IdPotion = id;
            drop.name = itemName;
        }

        public virtual void DoDamage(Transform obj, float dmg)
        {
            DamageText(dmg, _pl.Stats.critDamage, _pl.Stats.critChance);
            _hp -= dmg;
            if (_hp <= 0)
            {
                _pl.GetExp(_experience);

                _dead = true;
                if (_trfm.CompareTag("Boss")) _anim.SetBool("dead", _dead);
                else Die(obj);
            }
        }

        public virtual void DamageText(float dmg, float critDmg, float critChance) // вывод урона пули
        {
            float crit = Random.Range(0, 101);
            GameObject obj = new GameObject(); // создаем текст урона]
            obj.AddComponent<DamageUI>();
            var _dmgText = obj.GetComponent<DamageUI>();
            _dmgText.TargetPos = _trfm; // задаем позицию текста

            if (crit <= critChance)
            {
                dmg += dmg * critDmg / 100;
                _dmgText.Crit = true;
            }
            else
            {
                _dmgText.Crit = false;
            }
            _dmgText.Dmg = dmg;
        }

        public virtual void Die(Transform obj)
        {
            _agent.enabled = false;
            // включаем рэгдол элементы
            foreach (Collider c in _trfm.GetComponentsInChildren<Collider>())
            {
                c.isTrigger = false;
            }
            foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>())
            {
                r.isKinematic = false;
            }
            _anim.enabled = false;
            DropCheck();
            _col.enabled = false;
            Invoke(nameof(ColliderOff), 7f); // отключаем рэгдол элементы
        }

        public virtual void ColliderOff()
        {
            _col.enabled = false;
        }

        public virtual void RigOff()
        {
            foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>())
            {
                r.isKinematic = true;
            }
        }

        public virtual void DmgReset()
        {
            _isDmgDone = false;
        }

        public virtual void HitOff()
        {
            _isHit = false;
            _anim.SetBool("hit", false);
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            if (_dead || _isDmgDone) return;
            _isDmgDone = true;
            if (col.CompareTag("FrostSpell"))
            {
                DoDamage(col.transform.root, _playerSpells[1].dmg);
            }
            if (col.CompareTag("LightingSpell"))
            {
                DoDamage(col.transform.root, _playerSpells[2].dmg);
                Stunned();
            }
            if (col.CompareTag("Weapon"))
            {
                if (_pl.AttackType == 0) DoDamage(col.transform.root, _playerSpells[0].dmg);
                if (_pl.AttackType == 1) DoDamage(col.transform.root, _playerSpells[_playerSpells.Length - 2].dmg);
                if (_pl.AttackType == 2) DoDamage(col.transform.root, _playerSpells[_playerSpells.Length - 1].dmg);
                _isHit = true;
                _anim.SetBool("hit", true);
            }
            Invoke(nameof(DmgReset), 0.1f);
        }
    }
}
