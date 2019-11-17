using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game 
{
    public class PlayerModel : PlayerSettings
    {
        #region Control
        public void Roll() // кувырок
        {
            if (AttackType != 0) return; // если в режиме атаки, выходим
            if (_spellNow != 0) return; // если используем способности, выходим

            _isFreezeRot = false; // замораживаем повороты во время кувырка

            if (_move != 0)
            {
                _anim.SetInteger("roll", _move); // кувырок вперед
                _rig.velocity = _move * 10 * _trfm.forward;
            }
            else
            {
                _anim.SetInteger("roll", -1); // кувырок назад
                _rig.velocity = -1 * 10 * _trfm.forward;
            }
            _isRoll = true;
        } 
        public void RollEnd() // окончание кувырка
        {
            _rig.velocity = Vector3.zero;
            _anim.SetInteger("roll", 0);
            _isRoll = false;
        }
        public void UsePotion() // используем активную банку если она есть
        {
            if (ActivePotion == 0 && Potion[0] != 0) 
            {
                HealthPotion(ActivePotion, Item[ActivePotion].feature1);
            }
            else if (ActivePotion == 1 && Potion[1] != 0)
            {
                EnergyPotion(ActivePotion, Item[ActivePotion].feature1, Item[ActivePotion].feature2, Item[ActivePotion].feature3);
            }
            else if (ActivePotion == 2 && Potion[2] != 0)
            {
                PowerPotion(ActivePotion, Item[ActivePotion].feature1, Item[ActivePotion].feature3);
            }
        }
        public void UsePotion(int i)
        {
            ActivePotion = i - 1;
            UsePotion();
        }
        public void LockLook(bool _look) // перестать следить за курсором
        {
            _lockLook = _look;
        }
        public void Moving(float speed) // передвижение
        {
            if (AttackType != 0 || _isFreezeRot || _isRoll) return; // если атакую, не могу поворачиваться или кувыркаюсь, выхожу

            if (Input.GetAxis("Vertical") >= 0.2f) // вперед
            {
                _anim.SetInteger("move", 1);
            }
            else if (Input.GetAxis("Vertical") <= -0.2f) // назад
            {
                speed /= 2;
                _anim.SetInteger("move", -1);
            }
            else // стою
            {
                _anim.SetInteger("move", 0); 
            }
            _move = _anim.GetInteger("move");

            if (Input.GetAxis("Vertical") < 0.2f && Input.GetAxis("Vertical") > -0.2f) speed = 0; // стоим
            else if (Input.GetAxis("Vertical") != 0) // идем
            {
                _trfm.position += Input.GetAxis("Vertical") * (_trfm.forward * Time.deltaTime * speed);
            }
        }
        public void PlayerLook() // слежение за мышкой если поворот не заморожен
        {
            if (_isFreezeRot || _lockLook) return;
            PlayerLook(_trfm, _camera.ScreenPointToRay(Input.mousePosition));
        }
        public void GroundDistance() //проверяем есть ли под ногами пол
        {
            if (_checkGroundDist)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, _distForGround, out hit, 1))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        _anim.SetBool("freeze", false);
                        _anim.SetBool("groundSoon", true);
                        _checkGroundDist = false;
                    }
                }
            }
        }
        protected void PlayerLook(Transform trfm, Ray ray) // слежение персонажа за курсором
        {
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, 100, LayerMask.GetMask("Place")))
            {
                Vector3 cursor = _hit.point;
                cursor.y = trfm.position.y;
                trfm.rotation = Quaternion.LookRotation(cursor - trfm.position);
            }
        }
        protected void GroundDistCheck()
        {
            _checkGroundDist = true;
            _anim.SetBool("freeze", true);

        }
        protected void MiniCharge() // небольшой рывок вперед
        {
            _rig.velocity = transform.forward * 5;
        }
        protected void Jump() //прыжок вперед
        {
            _rig.velocity = ((transform.forward * 5) + (transform.up * 5));
        }
        #endregion

        #region Attack
        public void MainAttack(int num) // атака
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Attack(num);
        }
        protected void Attack(int num) // атака
        {
            if (_isRoll) return; // если кувыркаемся не атакуем
            if (_spellNow != 1) // атакуем если не используем способность с прыжком
            {
                _speed = _basicSpeed;
                SpellInterrupt(); // прерывание предыдущего спела если он был
                if (num == 1 && AttackType != 2) // легкая атака
                {
                    if (_comboLeftClick == 1) _comboLeftClick = 2;
                    _anim.SetInteger("comboLC", _comboLeftClick);
                    _anim.SetInteger("attack", num);
                    AttackType = num;
                }
                else if (num == 2) // тяжелая атака
                {
                    if (_isBuffOn) SpellBook[SpellBook.Length - 1].cost = 0; // если под бафом, спобосность бесплатна
                    else SpellBook[SpellBook.Length - 1].cost = _defaultAttackCost;

                    if (_playerUi.EnBar.fillAmount * 100 < SpellBook[SpellBook.Length - 1].cost) return; // если энергии не хватает на удар, выходим

                    if (_comboRightClick == 1) _comboRightClick = 2;
                    _anim.SetInteger("comboRC", _comboRightClick);
                    _anim.SetInteger("attack", num);
                    AttackType = num;
                }
            }
        }
        protected void AttackEffect() // эфекты атак
        {
            GameObject _attEff = null;
            if (_spellNow == 1) // если используем спел с прыжком, создаем под ногами лед на заданное время
            {
                _attEff = Instantiate(SpellBook[1].spell,
                                      SpellBook[1].spell.transform.position,
                                      SpellBook[1].spell.transform.rotation);
                Destroy(_attEff, 2.5f);
            }
            else if (AttackType == 2 || _isBuffOn) // если используем тяжелые атаки или находимся под бафом, создаем огненные волны при ударах
            {
                _attEff = Instantiate(_effects[0], _effects[0].transform.position, _effects[0].transform.rotation, _effects[0].transform.parent);
                _attEff.SetActive(true);
                Destroy(_attEff, 1f);
            }
            if (_attEff != null) _attEff.SetActive(true);
        }
        protected void AttackStart() // начало атаки
        {
            if (_isBuffOn && AttackType == 1) // если легкая атака под баффом, бьем усиленно 
            {
                AttackEffect();
            }
            else if (AttackType == 2) // тратим энергию на удар
            {
                _playerUi.ChangeValueEnergy(SpellBook[SpellBook.Length - 1].cost);
            }
        }
        protected void AttackEnd() // окончание атаки
        {
            AttackType = 0;
            _anim.SetInteger("attack", AttackType);
            _comboLeftClick = _comboRightClick = 0;
            _anim.SetInteger("comboLC", _comboLeftClick);
            _anim.SetInteger("comboRC", _comboRightClick);
        }
        protected void ComboReset() // сброс стрика атак
        {
            _comboLeftClick = _comboRightClick = 1;
            _anim.SetInteger("comboLC", _comboLeftClick);
            _anim.SetInteger("comboRC", _comboRightClick);
        }
        #endregion

        #region Spells
        public void Spell() // состояние спеллов
        {
            if (_spellNow == 0)
            {
                _speed = 0;
                _isFreezeRot = false;
                _trfm.position += _trfm.forward * (7 * Time.deltaTime);
                SpellBook[0].spell.SetActive(true);
            }
            if (_spellNow == 1 || _spellNow == 2)
            {
                _isFreezeRot = true;
                _speed = 0;
            }
            if (_isBuffOn) // если игрок под бафом
            {
                if (Timer(SpellBook[3].time)) Buff(); // окончание бафа через заданное время в таймере
                if (_trfm.localScale.y < _buffScale.y) // увеличение в размерах игрока
                {
                    _trfm.localScale += _scaleUpStep;
                }
                SpellBook[3].spell.SetActive(true); // включение эффекта бафа
                if (_spellNow != 0) _speed = 5;
            }
            if (!_isBuffOn)  // если игрок не под бафом
            {
                if (_trfm.localScale.y > _defaultScale.y) // уменьшается игрок до обычных размеров
                {
                    _trfm.localScale -= _scaleUpStep;
                    SpellBook[3].spell.SetActive(false);
                }
            }
        }
        public void SpellCast(int id) // каст спелла
        {
            if (SpellBook[id].status) return;
            if (_isRoll || _spellLast == 3) return; // если в кувырке или предыдущий спел был бафом, выходми
            if (_spellLast != 1 && AttackType == 0) // если предыдущий спел был прыжок и не проводится атака, кастуем спел
            {
                if (_playerUi.EnBar.fillAmount * 100 < SpellBook[id].cost) return;
                _playerUi.ChangeValueEnergy(SpellBook[id].cost);
                if (id == 3 && !_isBuffOn) // если спел это баф и игрок еще не бафнут
                {
                    _barUi.Cooldown(SpellBook[id]);
                    SpellInterrupt(); // прерывыем предыдущий спел если нужно
                    _isFreezeRot = true;
                    _spellLast = id;
                    _spellNow = id;
                    _anim.SetInteger("spell", id+1);
                    return; // выходим после получения бафа
                }
                if (_spellLast != 3) SpellInterrupt(); // если предыдущий спел не баф, прерываем его

                if (id == 0)
                {
                    SpellBook[id].spell.SetActive(true);
                }

                if (id != 1) // заканчиваем способность через заданное время если это не прыжок
                {
                    Invoke("SpellEnd", SpellBook[id].time);
                }
                _barUi.Cooldown(SpellBook[id]);
                _spellNow = _spellLast = id;
                _anim.SetInteger("spell", id+1); //активруем анимацию текущего спела
            }
        }
        public void SpellInterrupt() // прерывание предыдущей способности
        {
            CancelInvoke("SpellEnd");  
            if (_spellLast == 2) CancelInvoke("Lightings"); 
            if (_spellLast != 1) // если прердыдущая способность не прыжок, отменяем заморозку
            {
                _isFreezeRot = false;
                _anim.SetBool("spellCasting", false);
            }
            if (_spellNow != -1)
            {
                _spellNow = -1;
                _anim.SetInteger("spell", 0);
                if (_spellLast != -1) SpellBook[_spellLast].spell.SetActive(false); // прерываем эффект прошлого заклинания
            }
        }
        public void SpellEnd() // окончание способности
        {
            _speed = _basicSpeed;
            _isFreezeRot = false;
            _anim.SetBool("groundSoon", false);
            _anim.SetBool("spellCasting", false);
            if (_spellLast == 1) _spellLast = 0;
            if (_spellLast == 3) _spellLast = 0;
            if (_spellNow != -1)
            {
                SpellBook[_spellNow].spell.SetActive(false);
                _spellNow = -1;
            }
            _anim.SetInteger("spell", 0);
        }
        public void Casting() // каст способности в нужный момент
        {
            if (_spellNow == 2) // если способность с ударами молний, то выпускаем молнии во врагов через заданное время
            {
                SpellBook[2].spell.SetActive(true);
                Invoke("Lightings", 1);
            }
            _anim.SetBool("spellCasting", true);
        }
        public void Buff() // вкл/откл бафа
        {
            if (_isBuffOn) _isBuffOn = false;
            else
            {
                _isBuffOn = true;
                _anim.SetInteger("spell", 0);
            }
        }

        private void Lightings() // пускание молний
        {
            if (_spellNow != 2) return; // если способность не молнии, выходим
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 10, Quaternion.identity, LayerMask.GetMask("Enemy")); // Делаем оверлап зону в которой пускаем молнии во всех найденных врагов

            for (int i = 0; i < hitColliders.Length; i++) // проходимся по врагам
            {
                // да разок на старте способности создаю вектор на основе текущей цели и с помощью lossyScale.y / 2 бью именно до пола во врага
                Vector3 targetPos = new Vector3(hitColliders[i].transform.position.x,
                                                hitColliders[i].transform.position.y - hitColliders[i].transform.lossyScale.y / 2,
                                                hitColliders[i].transform.position.z);

                GameObject lighting = Instantiate(_effects[1], targetPos, _effects[1].transform.rotation);
                lighting.SetActive(true);
                Destroy(lighting, 3);
            }
        }
        #endregion
    }
}
