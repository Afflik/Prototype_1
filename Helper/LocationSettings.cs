using UnityEngine;

namespace Game
{
    public class LocationSettings : MainAbs
    {
        [SerializeField]
        private GameObject _fireWall;
        private GameObject _fireWalls;

        private void Start()
        {
            _fireWalls = new GameObject();
            _fireWalls.tag = "BossZone";
            _fireWalls.name = "FireWalls";

            Invoke("Starting", 0.01f);
        }

        private void Starting()
        {

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BossEnter"))
            {
                if (obj.transform.childCount == 0)
                {
                    GameObject wall;
                    wall = Instantiate(_fireWall, obj.transform.position + (obj.transform.forward * 2), obj.transform.rotation);
                    wall.transform.SetParent(_fireWalls.transform);
                }
            }
        }
    }
}
