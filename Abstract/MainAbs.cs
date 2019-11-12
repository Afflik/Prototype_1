using UnityEngine;

namespace Game
{
    public abstract class MainAbs : MonoBehaviour
    {

        protected Transform _trfm;
        protected GameObject _obj;
        protected Rigidbody _rig;
        protected Animator _anim;

        protected float _time = 0;

        protected Camera _camera;

        virtual public void Awake()
        {
            _camera = Camera.main;
            _obj = GetComponent<GameObject>();
            _trfm = GetComponent<Transform>();
            _rig = GetComponent<Rigidbody>();
            _anim = GetComponent<Animator>();
        }

        public virtual void SetTime(float t)
        {
            _time = t;
        }

        public virtual bool Timer(float t) // таймер
        {
            _time += Time.deltaTime;
            if (_time > t)
            {
                _time = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
