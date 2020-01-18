using UnityEngine;

namespace Game
{
    public class Fog : MainAbs, IOnUpdate
    {
        private Vector3 _pos;
        private Transform _playerPos;
        private void Start()
        {
            _pos = _trfm.position;
            _playerPos = FindObjectOfType<PlayerModel>().transform;
        }
        public void OnUpdate()
        {
            _pos = _playerPos.position;
            _pos.y = _playerPos.position.y - 28;
            _trfm.position = _pos;
        }
    }
}
