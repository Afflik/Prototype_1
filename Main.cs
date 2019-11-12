using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class Main : MonoBehaviour
    {
        private readonly List<IOnUpdate> _updates = new List<IOnUpdate>();

        public PlayerController PlayerCont { get; private set; }

        public GameObject Player { get; private set; }

        public static Main Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            PlayerCont = new PlayerController();
            _updates.Add(PlayerCont);
        }

        void Start()
        {
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
