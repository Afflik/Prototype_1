using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace Game
{
    public class ActionBarUI : MainAbs, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField]
        private Transform _spellBar;
        [SerializeField]
        private Transform _attackBar;
        [SerializeField]
        private Transform _potionBar;
        [SerializeField]
        private Transform _buffBar;

        private Transform _potion;
        private int _changePosPotion;

        private bool _isPotionMenuActive;
        private Vector3 _newPosPotion;
        private Vector3 _lastPosPotion;
        private Vector3 _buffStep;
        private Vector3 _buffStepDefault;

        private TextMeshProUGUI _count;

        private PlayerModel _pl;
        private PlayerUI _plUi;

        private Cooldowner _cdwner;

        private Transform _timer;
        private Image _progressTimerBar;
        private Image _cdImg;

        private Color _default;
        private Color _noEnergy;

        private List<Image> _spells = new List<Image>();
        private int[] _potions = new int[3];

        public TextMeshProUGUI PosionCount { get => _count; set => _count = value; }
        protected int[] Potion { get => _potions; set => _potions = value; }

        private void Start()
        {
            _buffStep = new Vector3(75, 0, 0);
            _buffStepDefault = _buffStep;
               _newPosPotion = new Vector3(0, 100, 0);

            PosionCount = GetComponent<TextMeshProUGUI>();
            _pl = FindObjectOfType<PlayerModel>();
            _plUi = FindObjectOfType<PlayerUI>();

            _default = new Color(0.78f, 0.62f, 0.21f); // стандартный цвет иконки способности
            _noEnergy = new Color(0.4f, 0.1f, 0.1f); // когда не хватает энергии цвет иконки способности

            foreach (Transform spell in _spellBar) // список способностей
            {
                spell.gameObject.AddComponent<Cooldowner>(); // добавляем кулдаун способностям
                _spells.Add(spell.GetChild(0).GetComponent<Image>());
            };
            _spells.Add(_attackBar.GetChild(1).GetChild(0).GetComponent<Image>());
            InvokeRepeating("EnergyCost", 0, 0.1f); // проверка на хватку энергии на способности
        }

        public void AddBuff(int id,float time) // добавление иконки баффа
        {
            var buff = Instantiate(_buffBar.GetChild(0), _buffBar.GetChild(0).position, Quaternion.identity, _buffBar);
            if(_buffBar.childCount > 2) // если баффов больще чем один, то добавляем бафф рядом 
            {
                buff.localPosition += _buffStep;
                _buffStep.x += (_buffBar.GetChild(buff.GetSiblingIndex()).localPosition.x - _buffBar.GetChild(buff.GetSiblingIndex()-1).localPosition.x);
            }
            buff.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/icons/" + id);
            buff.gameObject.SetActive(true);
            StartCoroutine(RemoteBuff(buff, time));
        }
        public IEnumerator RemoteBuff(Transform obj, float time) // офф иконки баффа
        {
            yield return new WaitForSeconds(time);
            for (int i = obj.GetSiblingIndex() + 1; i < _buffBar.childCount; i++)
            {
                _buffBar.GetChild(i).localPosition -= _buffStepDefault;
            }
            _buffStep = _buffStepDefault;
            Destroy(obj.gameObject);
            StopCoroutine("RemoteBuff");
        }

        public void AddPotion(int id) // добавить банку в инвентарь банок
        {
            _pl.ActivePotion = id;
            _potion = _potionBar.GetChild(id + 1);
            PosionCount = _potion.GetChild(_potion.childCount - 1).GetComponent<TextMeshProUGUI>();

            Potion[id]++; // плюс определенная банка
            PosionCount.text = Potion[id].ToString();

            _lastPosPotion = _potion.localPosition; // позиция в инвентаре текущей банки

            for (int i = 1; i < _potion.parent.childCount; i++)
            {
                if (_potion.parent.GetChild(i).localPosition == Vector3.zero) // если уже есть банка в активном окне
                {
                    _potion.parent.GetChild(i).localPosition = _lastPosPotion; // передвигаем ее место подобранной банки
                    _pl.ActivePotion = _potion.GetSiblingIndex() - 1; // меняем активную банку
                }
                _potion.parent.GetChild(i).gameObject.SetActive(false);
            }
            _potion.gameObject.SetActive(true); // отображаем подобранную банку
            _potion.localPosition = Vector3.zero; // переносим банку в активное окно
            _potion.GetChild(_potion.childCount - 2).gameObject.SetActive(false); // скрываем иконку отстуствия банки

        }
        public void RemotePotion(int id) // минус банка
        {
            Potion[id]--;
            _potion = _potionBar.GetChild(id + 1);
            PosionCount = _potion.GetChild(_potion.childCount - 1).GetComponent<TextMeshProUGUI>();

            if (Potion[id] == 0) // если банки кончились
            {
                for (int i = 1; i < _potionBar.childCount; i++) // пробегаемся по всем банкам
                {
                    if (!_potionBar.GetChild(i).GetChild(4).gameObject.activeSelf) // если найдена в инвентаре другая банка
                    {
                        if (_potionBar.GetChild(i).localPosition == _newPosPotion * 2) // если она на последнем месте
                        {
                            SwitchPotion(i, _newPosPotion * 2); // переносим ее в активное окно вместо той которая кончилась
                            break; // перестаем дальше чекать банки
                        }
                        else if (_potionBar.GetChild(i).localPosition == _newPosPotion) // если остался один тип банки
                        {
                            SwitchPotion(i, _newPosPotion); // переносим ее в активное окно вместо той которая кончилась
                            break; // перестаем дальше чекать банки
                        }
                    }
                }
                PosionCount.text = " "; // убираем количество банок, которые кончились
                _potion.GetChild(4).gameObject.SetActive(true); 
                _potion.gameObject.SetActive(false);
            }
            else
            {
                PosionCount.text = Potion[id].ToString(); // если банка использована и она была не последней, просто уменьшаем количество
            }
        }

        public void SwitchPotion(int id, Vector3 pos) // смена активной банки если активные кончились 
        {
            _pl.ActivePotion = id - 1; // смена активной банки
            _potion.localPosition += pos; // новая позиция ячейки пустых банок
            _potionBar.GetChild(id).localPosition = Vector3.zero; // новая активная банка
            _potionBar.GetChild(id).gameObject.SetActive(true); // включение банки
        } 

        public void Cooldown(SpellManager.Spell spell) // настраиваем кулдаун/полоску прогресса для спела
        {
            _cdImg = _spellBar.GetChild(spell.id).GetChild(1).GetComponent<Image>(); // иконка кд способности

            if (spell.isBuff) // если способность бафф
            {
                _timer = _spellBar.GetChild(spell.id).GetChild(_spellBar.GetChild(spell.id).childCount - 1);
                if (!_timer.GetComponent<Cooldowner>()) _timer.gameObject.AddComponent<Cooldowner>();

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
            for (int i = 0; i < _spells.Count; i++)
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

        private void RefreshPotionMenu() // обновление позиций банок при переставлении
        {
            for (int i = 0; i < _potionBar.childCount; i++)
            {
                if (_potionBar.GetChild(i).localPosition.y != 0) _potionBar.GetChild(i).gameObject.SetActive(_isPotionMenuActive); 
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) // если нажали левой кнопкой мышки
            {
                Transform potion = eventData.rawPointerPress.transform.parent; // сохраняем элемент который нажали
                if (Potion[potion.GetSiblingIndex() - 1] == 0) return; // если нажали в пустую ячейку, ничего не происходит
                if (potion.localPosition == Vector3.zero) // если нажали по активной банке мышкой, используем ее
                {
                    _pl.UsePotion(potion.GetSiblingIndex());
                    RefreshPotionMenu();
                }
                else // если нажали по банке в инвентаре
                {
                    _lastPosPotion = potion.localPosition;

                    for (int i = 1; i < _potionBar.childCount; i++)
                    {
                        if (_potionBar.GetChild(i).localPosition == Vector3.zero) // убираем активную банку в инвентарь
                        {
                            _potionBar.GetChild(i).localPosition = _lastPosPotion;
                        }
                        _potionBar.GetChild(i).gameObject.SetActive(false);
                    }
                    potion.gameObject.SetActive(true);
                    potion.localPosition = Vector3.zero; // задаем активной ту банку, на которую нажали в инвентаре
                    _pl.ActivePotion = potion.GetSiblingIndex() - 1;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right) // если нажали правой кнопкой мыши по окошку банок, открываем инвентарь
            {
                if (_isPotionMenuActive) _isPotionMenuActive = false;
                else _isPotionMenuActive = true;
                RefreshPotionMenu();
            }
        } // реакция на клик 

        public void OnPointerExit(PointerEventData eventData) // реакция при отведении мышки от элемента
        {
            _isPotionMenuActive = false;
            RefreshPotionMenu(); // скрываем меню банок если отвели мышку с открытого инвентаря
        } 
    }
}
