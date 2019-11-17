using UnityEngine;

namespace Game
{
    public class EnemyModel : MainAbs, IOnUpdate
    {
        private PlayerModel _pl;

        protected float _hp = 1000;

        protected SpellManager.Spell[] _playerSpells;
        protected ItemData _itemList;

        protected Collider _col;

        protected bool _dead;

        protected ItemData.Item[] DropList { get => _itemList.Items; set => _itemList.Items = value; }

        private new void Awake()
        {
            base.Awake();
            _pl = FindObjectOfType<PlayerModel>();
            _col = gameObject.GetComponent<Collider>();
            _itemList = FindObjectOfType<ItemData>();
            _playerSpells = FindObjectOfType<SpellManager>().Spells;
            
            foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>()) // отключение физики для рэгдол элементов
            {
                r.isKinematic = true;
            }
        }

        public void DropCheck() // дроп баночек
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

        public void Drop(int id, GameObject item) // если шанс сработал, с врага падает банка
        {
            string itemName = item.name;
            var drop = Instantiate(item, _trfm.position, Quaternion.identity);
            drop.GetComponent<Potion>().IdPotion = id;
            drop.name = itemName;
        }

        public void DoDamage(Transform obj, float dmg)
        {
            _hp -= dmg;
            if (_hp < 1)
            {
                _dead = true;
                Die(obj);
            }
        }

        public void Die(Transform obj)
        {
            // включаем рэгдол элементы
            foreach (Collider c in _trfm.GetComponentsInChildren<Collider>()) 
                {
                    c.isTrigger = false;
            }
            foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>())
            {
                r.isKinematic = false;
            }
            _rig.AddForceAtPosition(Vector3.up * 10, obj.position, ForceMode.Impulse); 
            _anim.enabled = false;
            DropCheck();
            Invoke("ColliderOff", 0.5f); // отклчюаем колайдер врага
            Invoke("Death", 7f); // отключаем рэгдол элементы
        }

        public void ColliderOff()
        {
            _col.enabled = false;
        }

        public void Death()
        {
            foreach (Rigidbody r in _trfm.GetComponentsInChildren<Rigidbody>())
            {
                r.isKinematic = true;
            }
        }

        public void OnUpdate()
        {
        }

        private void OnTriggerEnter(Collider col)
        {
            if (_dead) return;
            if (col.CompareTag("FrostSpell"))
            {
                DoDamage(col.transform.root, _playerSpells[1].dmg);
            }
            if (col.CompareTag("LightingSpell"))
            {
                DoDamage(col.transform.root, _playerSpells[2].dmg);
            }
            if (col.CompareTag("Weapon"))
            {
                if(_pl.AttackType == 0)  DoDamage(col.transform.root, _playerSpells[0].dmg);
                if (_pl.AttackType == 1) DoDamage(col.transform.root, _playerSpells[_playerSpells.Length - 2].dmg);
                if (_pl.AttackType == 2) DoDamage(col.transform.root, _playerSpells[_playerSpells.Length - 1].dmg);
            }
        }
    }
}
