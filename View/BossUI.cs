using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game
{
    public class BossUI : MainAbs
    {
        public Image HpBar { get; set; }

        private void Start()
        {
            HpBar = transform.GetChild(0).GetComponent<Image>();
        }

        public void BossName(string name)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = name;
        }

        public void GetDamage(float hp, float maxHp)
        {
            HpBar.fillAmount = hp / maxHp;
        }
    }
}
