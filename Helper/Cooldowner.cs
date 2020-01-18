using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Cooldowner: MonoBehaviour
    {
        private SpellManager.Spell _spell; // загрузка шаблона способности
        private ItemData.Item _item; // загрузка шаблона предметов
        private PlayerModel _pl;
        private Image _img;

        private float _time;
        private float _tick = 0.05f;
        private bool _isProgressBar;
        private bool _isSpell;
        
        public void StartWait(SpellManager.Spell spell, Image img, bool isPrBar) // передача конкретной способности и проверка кд способности или продолжительность спобосности
        {
            _isSpell = true;
           _pl = FindObjectOfType<PlayerModel>();

            _isProgressBar = isPrBar;

            _img = img;
            _spell = spell;
            if (_isProgressBar) _time =  _spell.time; // если это индикатор продолжительности, то считаем сколько осталось временя действия
            else
            {
                _time = _spell.cd; // если это кд, то считаем когда спобосность снова будет в доступе
                _pl.SpellBook[spell.id].status = true;
            }

            _img.gameObject.SetActive(true);
            InvokeRepeating(nameof(Cooldown), 0, _tick);
        }

        public void StartWait(ItemData.Item item, Image img, float cd) // кд для банки
        {
            _isSpell = false;
            _pl = FindObjectOfType<PlayerModel>();

            _img = img;
            _item = item;
            _time = cd;
            _pl.Item[_item.id].status = true;
            _img.gameObject.SetActive(true);
            InvokeRepeating(nameof(Cooldown), 0, _tick);
        }

            public void Cooldown()
        {
            if (_img.fillAmount > 0) _img.fillAmount -= _tick / _time;
            else
            {
                _img.fillAmount = 1;
                _img.gameObject.SetActive(false);
                if(_isSpell) _pl.SpellBook[_spell.id].status = false;
                else _pl.Item[_item.id].status = false;
                CancelInvoke(nameof(Cooldown));
            }
        }
    }
}
