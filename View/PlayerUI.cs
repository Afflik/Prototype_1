using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerUI : MainAbs
    {
        private float _spRestEnergy;
        private float _tick = 0.05f;

        private Image _hpBar;
        private Image _enBar;

        public Image HpBar { get => _hpBar; set => _hpBar = value; }
        public Image EnBar { get => _enBar; set => _enBar = value; }
        public float SpRestEnergy { get => _spRestEnergy; set => _spRestEnergy = value; }

        private void Start()
        {
            SpRestEnergy /= 100 / _tick;
            _hpBar = transform.GetChild(1).GetComponent<Image>();
            _enBar = transform.GetChild(2).GetComponent<Image>();
            InvokeRepeating("Status", 0, _tick);
        }

        public void ChangeValueHealth()
        {

        }
        public void ChangeValueEnergy(float cost)
        {
            _enBar.fillAmount -= cost / 100;
        }

        public void Status()
        {
            if(_enBar.fillAmount <= 1)
            {
             _enBar.fillAmount += SpRestEnergy;
            }
        }
    }
}
