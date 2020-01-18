using UnityEngine;

namespace Game
{
    public class Potion: MainAbs
    {
        [SerializeField]
        public int IdPotion = 1;

        private string _name;

        private ItemData.Item[] _potion;

        void Start()
        {
            _potion = FindObjectOfType<ItemData>().Items;

               _name = gameObject.name;
        }

        private void OnTriggerEnter(Collider col)
        {
            if(col.CompareTag("Player"))
            {
                for (int i = 0; i < 3; i++)
                {
                    if(_potion[i].id == IdPotion)
                    {
                        col.GetComponent<IPotion>().TakePotion(i);
                        break;
                    }
                }
                {
                    // col.GetComponent<IPotion>().HealthPotion(_healInPercent);
                }
                {
                   // col.GetComponent<IPotion>().EnergyPotion(_energyInPercent, _reduceCdInPercent, _boostTime);
                }
                gameObject.SetActive(false);
            }
        }
    }
}
