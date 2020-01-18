using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemySpawner : MainAbs
    {
        [SerializeField]
        private NavMeshSurface _navMesh;
        [SerializeField]
        private int _enemyCount = 50;
        [SerializeField]
        private GameObject[] _enemy;
        [SerializeField]
        private GameObject[] _boss;

        private GameObject[] _grounds;

        private Vector3 _spawnPos;
        private Vector3 _spawnRot;
        
        private bool _wrongSpawn;
        

        void Start()
        {

            _spawnRot = Vector3.zero;
            _spawnPos = Vector3.zero;;
            _navMesh.BuildNavMesh();
            Invoke("Spawn",0.01f);
        }

        private void Spawn()
        {
            Instantiate(_boss[0], GameObject.FindGameObjectWithTag("BossPlace").transform.position, Quaternion.identity).name = _boss[0].name;

            foreach (Transform trfm in _navMesh.transform)
            {
                if (   trfm.CompareTag("StartGame") 
                    || trfm.CompareTag("EndGame") 
                    || trfm.CompareTag("BossPlace")
                    || trfm.CompareTag("Connector"))
                    continue;
                
                int rng = Random.Range(4, 11);
                for (int i = 0; i <= rng; i++)
                {
                    _spawnRot.y = Random.Range(0, 36) * 10;
                    _spawnPos.y = 0.1f;
                    _spawnPos.x = Random.Range(0f, 2f);
                    _spawnPos.z = Random.Range(0f, 2f);

                    Instantiate(_enemy[0], trfm.position + _spawnPos, Quaternion.Euler(_spawnRot));
                }
            }
        }
    }
}
