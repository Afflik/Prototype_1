using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game
{
    public class PlayerUI : MainAbs, IPointerEnterHandler, IPointerExitHandler
    {
        private PlayerModel _pl;

        private float _spRestEnergyDefault; // стандартная скорость восстановление энергии
        private float _spRestEnergy; // скорость восстановление энергии
        private float _spRestHealth; // скорость восстановление энергии
        private float _tick = 0.05f; // скорость анимации полосок
        private float _heal;
        private float _energy;

        private bool _healing; // если хилимся
        private bool _charging; // если восстанавливаем энергию

        private Image _hpBar;
        private Image _enBar;
        private TextMeshProUGUI _hpAmount;
        private TextMeshProUGUI _enAmount;

        public Image HpBar { get => _hpBar; set => _hpBar = value; }
        public Image EnBar { get => _enBar; set => _enBar = value; }
        public float SpRestEnergy { get => _spRestEnergy; set => _spRestEnergy = value; }

        private void Start()
        {
            _pl = FindObjectOfType<PlayerModel>();
               SpRestEnergy /= 100 / _tick;
            _spRestEnergyDefault = SpRestEnergy;

            _hpBar = transform.GetChild(1).GetComponent<Image>();
            _hpAmount = _hpBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _hpAmount.gameObject.SetActive(false);

            _enBar = transform.GetChild(2).GetComponent<Image>();
            _enAmount = _enBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _enAmount.gameObject.SetActive(false);

            InvokeRepeating(nameof(Status), 0, _tick);
            
        }

        public void Health(float hp, float maxHp)
        {
            HpBar.fillAmount = hp / maxHp;
        }

        public void ChangeValueEnergy(float cost) // - энергия
        {
            _enBar.fillAmount -= cost / 100;
        }

        public void EnergyPotion(float energy, float time)
        {
            energy /= 100 / _tick;
            SpRestEnergy = energy;
            _charging = true;
            Invoke(nameof(RestoreEnergyOff), time);
        }

        public void RestoreHealth(float hp, float time)
        {
            hp /= 1000 / _tick;
            _spRestHealth = hp;
            _healing = true;
            Invoke(nameof(RestoreHealthOff), time);
        }

        public void RestoreEnergyOff()
        {
            _healing = false;
        }
        public void RestoreHealthOff()
        {
            _healing = false;
        }


        public void Status()
        {
            if (_enBar.fillAmount <= 1) // восстановление энергии
            {
                _enBar.fillAmount += SpRestEnergy;
                _pl.Energy = _enBar.fillAmount * _pl.MaxEnergy;
            }

            if (_healing)
            {
                _hpBar.fillAmount += _spRestHealth;
                _pl.Hp = _hpBar.fillAmount * _pl.MaxHp;
            }

            if (_charging) // ускоренное восстановление энергии
            {
                    SpRestEnergy = _spRestEnergyDefault;
            }
            _hpAmount.text = "" + (int)_pl.Hp;
            _enAmount.text = "" + (int)_pl.Energy;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
                _enAmount.gameObject.SetActive(true);
                _hpAmount.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hpAmount.gameObject.SetActive(false);
            _enAmount.gameObject.SetActive(false);
        }
    }
}
