using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Cooldowner: MonoBehaviour
    {
        private SpellManager.Spell _spell; // загрузка шаблона способности
        private PlayerModel _pl;
        private Image _img;

        private float _tick = 0.05f;
        private bool _isProgressBar;

        public void StartWait(SpellManager.Spell spell, Image img, bool isPrBar) // передача конкретной способности и проверка кд способности или продолжительность спобосности
        {
            _pl = FindObjectOfType<PlayerModel>();

            _isProgressBar = isPrBar;

            _img = img;
            _spell = spell;
            if(!_isProgressBar) _pl.SpellBook[spell.id].status = true; // если это не индикатор продолжительности
            _img.gameObject.SetActive(true);
            InvokeRepeating("Cooldown", 0, _tick);
        }
        public void Cooldown()
        {
            if (_img.fillAmount > 0 && _isProgressBar) _img.fillAmount -= _tick / _spell.time; // если это индикатор продолжительности, то считаем сколько осталось временя действия
            else if (_img.fillAmount > 0 && !_isProgressBar) _img.fillAmount -= _tick / _spell.cd; // если это кд, то считаем когда спобосность снова будет в доступе
            else
            {
                _img.fillAmount = 1;
                _img.gameObject.SetActive(false);
                _pl.SpellBook[_spell.id].status = false;
                CancelInvoke("Cooldown");
            }
        }
    }
}
