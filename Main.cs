using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class Main : MonoBehaviour
    {
        private readonly List<IOnUpdate> _updates = new List<IOnUpdate>();

        public PlayerController PlayerCont { get; private set; }
        public GameObject Player { get; private set; }

        public EnemyModel[] Enemy { get; private set; }

        public static Main Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            PlayerCont = new PlayerController();
            Enemy = FindObjectsOfType<EnemyModel>();
            _updates.Add(PlayerCont);
        }

        void Start()
        {
            PlayerCont.Init();
            foreach (EnemyModel enemy in Enemy)
            {

                _updates.Add(enemy);
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
