using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class MainCamera : MainAbs
    {

        [SerializeField] private float _smoothing = 5f; // степень плавности слежения
        [SerializeField] private Material _transp;     // материал прозрачности
        private List<List<Material>> _targetMat;      // лист листов материалов объекта. На всякий случай листов потому что вдруг их будет несколько на объекте.
        private List<Renderer> _objs;                // лист объектов
        private Transform _target;                  // цель для слежки
        private Vector3 _offset;                   // расстояние от цели до камеры
        private int _index = 0;                   // индекс элемента листа
        private int _isActive = 2;               // проверка состояния активности использования листа // лист объектов

        private void Start()
        {
            _objs = new List<Renderer>();
            _targetMat = new List<List<Material>>();
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _offset = _trfm.position - _target.position;
            transform.SetParent(null);
        }

        private void LateUpdate()
        {
            Follow(); // слежение за персонажем
        }

        private IEnumerator ClearActiveObj() // очисктка списка объектов  и материалов
        {
            yield return new WaitForSeconds(3);
            if (_isActive == 1)
            {
                StopAllCoroutines();
                yield break; // если прямо сейчас задействан список, прерывает корутину, если нет клирим листы
            }
            _objs.Clear();
            _targetMat.Clear();
            _index = 0;

            StopAllCoroutines();
        }

        private void Follow() // слежение за персонажем
        {
            Vector3 targetCamPos = _target.position + _offset;
            transform.position = Vector3.Lerp(_trfm.position, targetCamPos, _smoothing * Time.deltaTime);
        }

        public void OnTriggerEnter(Collider col)
        {
            if (!col.CompareTag("Player") && !col.CompareTag("Enemy"))                                                          // если объект не игрок, делает его прозрачным
            {
                _objs.Add(col.GetComponent<Renderer>());                                          // добавляем объект в лист объектов
                _targetMat.Add(_objs[_index].sharedMaterials.Cast<Material>().ToList());         // добавляем материалы объекта в лист листов материалов
                Material[] sharedMaterials = (Material[])_objs[_index].sharedMaterials.Clone(); // клонируем список материалов объекта в новый список
                for (int i = 0; i < sharedMaterials.Count(); i++)                              // пробегаемся по материалам, делая их прозрачными
                {
                    sharedMaterials[i] = new Material(_transp);
                }
                _objs[_index].sharedMaterials = sharedMaterials;   // объект прозрачный
                _index++;                                         // переход к след месту из списка объектов
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (!col.CompareTag("Player") && !col.CompareTag("Enemy"))
            {
                _isActive = 1; // переменная использования списка
            }
        }

        public void OnTriggerExit(Collider col)
        {
            if (!col.CompareTag("Player") && !col.CompareTag("Enemy"))
            {
                _objs[_objs.IndexOf(col.GetComponent<Renderer>())].sharedMaterials = _targetMat[_index - 1].ToArray(); // на выходе из зоны объекта, возвращаем ему прежние материалы
                _isActive = 0;
            }
            StartCoroutine(ClearActiveObj()); // запускаем корутину отчистки
        }
    }
}
