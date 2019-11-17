using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerUI : MainAbs
    {
        private float _spRestEnergyDefault; // стандартная скорость восстановление энергии
        private float _spRestEnergy; // скорость восстановление энергии
        private float _tick = 0.05f; // скорость анимации полосок
        private float _heal;
        private float _energy;

        private bool _healing; // если хилимся
        private bool _charging; // если восстанавливаем энергию

        private Image _hpBar;
        private Image _enBar;

        public Image HpBar { get => _hpBar; set => _hpBar = value; }
        public Image EnBar { get => _enBar; set => _enBar = value; }
        public float SpRestEnergy { get => _spRestEnergy; set => _spRestEnergy = value; }

        private void Start()
        {
            SpRestEnergy /= 100 / _tick;
            _spRestEnergyDefault = SpRestEnergy;
            _hpBar = transform.GetChild(1).GetComponent<Image>();
            _enBar = transform.GetChild(2).GetComponent<Image>();
            InvokeRepeating("Status", 0, _tick);
            
        }

        public void ChangeValueHealth(float hp) // + хил
        {
            HpBar.fillAmount += hp;
        }

        public void ChangeValueEnergy(float cost) // - энергия
        {
            _enBar.fillAmount -= cost / 100;
        }

        public void HealthPotion(float heal)
        {
            _heal = heal / 100;
            _healing = true;
        }
        public void EnergyPotion(float energy, float time)
        {
            energy /= 100 / _tick;
            SpRestEnergy = energy;
            SetTime(time);
            _charging = true;
        }

            public void Status()
        {
            if(_enBar.fillAmount <= 1) // восстановление энергии
            {
             _enBar.fillAmount += SpRestEnergy;
            }
            

            if (_charging) // ускоренное восстановление энергии
            {
                if (Timer())
                {
                    SpRestEnergy = _spRestEnergyDefault;
                       _charging = false;
                }
            }
        }
    }
}
