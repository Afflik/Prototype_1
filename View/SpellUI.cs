using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game
{
    public class SpellUI: MainAbs
    {
        private PlayerModel _pl;
        private PlayerUI _plUi;

        private  Cooldowner _cdwner;

        private Transform _timer;
        private Image _progressTimerBar;
        private Image _cdImg;

        private Color _default;
        private Color _noEnergy;

        private List<Image> _spells = new List<Image>();

        private void Start()
        {
            _pl = FindObjectOfType<PlayerModel>();
            _plUi = FindObjectOfType<PlayerUI>();

            _default = new Color(0.78f, 0.62f, 0.21f); // стандартный цвет иконки способности
            _noEnergy = new Color(0.4f, 0.1f, 0.1f); // когда не хватает энергии цвет иконки способности

            foreach (Transform spell in _trfm) // список способностей
            {
                if (spell == _trfm.GetChild(_trfm.childCount - 1))
                {
                    _spells.Add(spell.GetChild(1).GetChild(0).GetComponent<Image>());
                }
                else
                {
                    spell.gameObject.AddComponent<Cooldowner>(); // добавляем кулдаун способностям
                    _spells.Add(spell.GetChild(0).GetComponent<Image>());
                }
            };
            InvokeRepeating("EnergyCost", 0, 0.5f); // проверка на хватку энергии на способности
        }
        public void Cooldown(SpellManager.Spell spell) // настраиваем кулдаун/полоску прогресса для спела
        {
            _cdImg = _trfm.GetChild(spell.id).GetChild(1).GetComponent<Image>(); // иконка кд способности

            if (spell.isBuff) // если способность бафф
            {
                _timer = _trfm.GetChild(spell.id).GetChild(_trfm.childCount - 1);
                if(!_timer.GetComponent<Cooldowner>())_timer.gameObject.AddComponent<Cooldowner>();

                _progressTimerBar = _timer.GetChild(1).GetComponent<Image>();
                _timer.gameObject.SetActive(true);

                _cdwner = _timer.GetComponent<Cooldowner>();
                _cdwner.StartWait(spell, _progressTimerBar, true);
            }

            _cdwner = _cdImg.GetComponentInParent<Cooldowner>();
            _cdwner.StartWait(spell, _cdImg, false); // запускаем кулдаун для конкретного спела
        }

        private void EnergyCost() // если не хватает энергии, цвет иконки способности меняется
        {
           for(int i = 0; i < _spells.Count; i++)
            {
                if (_plUi.EnBar.fillAmount * 100 < _pl.SpellBook[i].cost)
                {
                    _spells[i].color = _noEnergy;
                }
                else
                {
                    _spells[i].color = _default;
                }
            }
        }
    }
}
