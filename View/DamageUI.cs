using UnityEngine;
using TMPro;

namespace Game
{
    public class DamageUI : MainAbs
    {
        private float _tickSpeed = 5;             // скорость движения текста
        public Transform TargetPos { get; set; }        // позиция текста
        public bool Crit { get; set; }               // проверка на крит
        public float Dmg { get; set; }                // урон

        private Vector3 _offsetUp;
        private Vector3 _currentPos;
        private Vector3 _offsetTick = new Vector3(0, 2, 0);
        private Vector3 _offset = new Vector3(0, 3, 0);

        private TextMeshProUGUI _text;
        private Color _color;
        private Color _default; // изначальный цвет
        private float _tick;

        private void Start()
        {
            transform.SetParent(FindObjectOfType<Canvas>().transform); // задаем Canvas родителем 
            gameObject.AddComponent<TextMeshProUGUI>();
            _text = GetComponent<TextMeshProUGUI>();
            _text.alignment = TextAlignmentOptions.Center;

            if (Crit) // если крит, цвет текста красный и шрифт немного больше
            {
                _text.color = _default = _color = Color.red;
                _text.fontSize = 40;
            }
            else     // если не крит, цвет текста желтый и шрифт обычный
            {
                _text.color = _default = _color = Color.yellow;
                _text.fontSize = 35;
            }
            _color.a = 0;
            _tick = 0.01f;
            
            TextPos();
            _text.text = Dmg.ToString();
            InvokeRepeating(nameof(DamageTextSettings), 0, 0.01f); // использую его, вместо того, чтобы добавлять апдейт каждому созданному тексту 
        }


        public void TextPos() // настройка позиции
        {
            _currentPos = _camera.WorldToScreenPoint(TargetPos.position + _offset);
            transform.position = _currentPos;
        }

        public void DamageTextSettings()
        {
            if (_text.color.a < 0.1f) Destroy(gameObject); // если текст стал прозрачным, уничтожаем его
            _currentPos.x = _camera.WorldToScreenPoint(TargetPos.position).x;
            _currentPos.z = _camera.WorldToScreenPoint(TargetPos.position).z;
            _currentPos.y += _offsetTick.y + (Time.deltaTime * _tickSpeed);
            _text.color = Color.Lerp(_default, _color, _tick); // плавное исчезновение текста
            transform.position = _currentPos;
            _tick += 0.01f;                                  // увеличиваем скорость исчезновения текста

        }
    }
}
