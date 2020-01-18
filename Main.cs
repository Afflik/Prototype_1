using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class Main : MonoBehaviour
    {
        private readonly List<IOnUpdate> _updates = new List<IOnUpdate>();

        public PlayerController PlayerCont { get; private set; }
        public GameObject Player { get; private set; }

        public EnemyMelee[] EnemyMelee { get; private set; }

        public static Main Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            PlayerCont = new PlayerController();
            _updates.Add(PlayerCont);
            _updates.Add(FindObjectOfType<Fog>());
        }

        void Start()
        {
            PlayerCont.Init();
            Invoke(nameof(AfterGen), 0.1f);
        }

        private void AfterGen()
        {
            _updates.Add(GameObject.FindGameObjectWithTag("Boss").GetComponent<IOnUpdate>());
            foreach (EnemyMelee em in FindObjectsOfType<EnemyMelee>())
            {
                _updates.Add(em);
            }
        }

        void Update()
        {
            for (var u = 0; u < _updates.Count; u++)
            {
                _updates[u].OnUpdate();
            }
        }
    }
}
