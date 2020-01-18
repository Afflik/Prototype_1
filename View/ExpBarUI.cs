using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class ExpBarUI : MainAbs
    {

        private Image _exp;
        private float _totalExp;

        void Start()
        {
            _exp = _trfm.GetChild(1).GetComponent<Image>();
        }

        public bool GetExp(float exp)
        {
            _totalExp += exp / 1000;
            if (_totalExp >= 1)
            {
                _totalExp = -1 * (1 - _totalExp);
                _exp.fillAmount = _totalExp;
                return true;
            }
            _exp.fillAmount = _totalExp;
            return false;
        }
    }
}
