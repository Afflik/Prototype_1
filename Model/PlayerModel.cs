using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerModel : PlayerSettings
    {
        #region Control
        public void Roll() // кувырок
        {
            if (_attackType != 0) return; // если в режиме атаки, выходим
            if (_spellNow != null) return; // если используем способности, выходим

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
        public void Moving(float speed) // передвижение
        {
            if (_attackType != 0 || _isFreezeRot || _isRoll) return; // если атакую, не могу поворачиваться или кувыркаюсь, выхожу

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
            if (!_isFreezeRot)
            {
                PlayerLook(_trfm, _camera.ScreenPointToRay(Input.mousePosition));
            }
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
        public void Attack(int num) // атака
        {
            if (_isRoll) return; // если кувыркаемся не атакуем
            if (_spellNow != _frostJumpSpell) // атакуем если не используем способность с прыжком
            {
                SpellInterrupt(); // прерывание предыдущего спела если он был
                if (num == 1 && _attackType != 2) // легкая атака
                {
                    if (_comboLeftClick == 1) _comboLeftClick = 2;
                    _anim.SetInteger("comboLC", _comboLeftClick);
                    _anim.SetInteger("attack", num);
                    _attackType = num;
                }
                else if (num == 2) // тяжелая атака
                {
                    if (_comboRightClick == 1) _comboRightClick = 2;
                    _anim.SetInteger("comboRC", _comboRightClick);
                    _anim.SetInteger("attack", num);
                    _attackType = num;
                }
            }
        }
        protected void AttackEffect() // эфекты атак
        {
            GameObject _attEff = null;
            if (_spellNow == _frostJumpSpell) // если используем спел с прыжком, создаем под ногами лед на заданное время
            {
                _attEff = Instantiate(_spellBook[_spellBook.IndexOf(_frostJumpSpell)],
                                      _spellBook[_spellBook.IndexOf(_frostJumpSpell)].transform.position,
                                      _spellBook[_spellBook.IndexOf(_frostJumpSpell)].transform.rotation);
                Destroy(_attEff, 2.5f);
            }
            else if (_attackType == 2 || _isBuffOn) // если используем тяжелые атаки или находимся под бафом, создаем огненные волны при ударах
            {
                _attEff = Instantiate(_effects[0], _effects[0].transform.position, _effects[0].transform.rotation, _effects[0].transform.parent);
                _attEff.SetActive(true);
                Destroy(_attEff, 1f);
            }
            if (_attEff != null) _attEff.SetActive(true);
        }
        protected void AttackStart() // начало атаки
        {
            if (_isBuffOn && _attackType == 1) // если легкая атака под баффом, бьем усиленно 
            {
                AttackEffect();
            }
        }
        protected void AttackEnd() // окончание атаки
        {
            _attackType = 0;
            _anim.SetInteger("attack", _attackType);
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
            if (_spellNow == _hurricaneSpell)
            {
                _speed = 0;
                _isFreezeRot = false;
                _trfm.position += _trfm.forward * (7 * Time.deltaTime);
                _spellBook[_spellBook.IndexOf(_hurricaneSpell)].SetActive(true);
            }
            if (_spellNow == _frostJumpSpell || _spellNow == _lightingStrikeSpell)
            {
                _isFreezeRot = true;
                _speed = 0;
            }
            if (_isBuffOn) // если игрок под бафом
            {
                if (Timer(_buffRampageTime)) Buff(); // окончание бафа через заданное время в таймере
                if (_trfm.localScale.y < _buffScale.y) // увеличение в размерах игрока
                {
                    _trfm.localScale += _scaleUpStep;
                }
                _spellBook[4].SetActive(true); // включение эффекта бафа
                if (_spellNow != _hurricaneSpell) _speed = 5;
            }
            if (!_isBuffOn)  // если игрок не под бафом
            {
                if (_trfm.localScale.y > _defaultScale.y) // уменьшается игрок до обычных размеров
                {
                    _trfm.localScale -= _scaleUpStep;
                    _spellBook[4].SetActive(false);
                }
            }
        }
        public void SpellCast(GameObject spell, float time) // каст спелла
        {
            if (_isRoll || _spellLast == _rampageBuff) return; // если в кувырке или предыдущий спел был бафом, выходми
            if (_spellLast != _frostJumpSpell && _attackType == 0) // если предыдущий спел был прыжок и не проводится атака, кастуем спел
            {
                if (spell == _rampageBuff && !_isBuffOn) // если спел это баф и игрок еще не бафнут
                {
                    SpellInterrupt(); // прерывыем предыдущий спел если нужно
                    _buffRampageTime = time; // задаем время бафу
                    _isFreezeRot = true;
                    _spellLast = spell;
                    _spellNow = spell;
                    _anim.SetInteger("spell", _spellBook.IndexOf(spell));
                    return; // выходим после получения бафа
                }
                if (_spellLast != _rampageBuff) SpellInterrupt(); // если предыдущий спел не баф, прерываем его

                if (spell == _hurricaneSpell)
                {
                    spell.SetActive(true);
                }

                if (spell != _frostJumpSpell) // заканчиваем способность через заданное время если это не прыжок
                {
                    Invoke("SpellEnd", time);
                }
                _spellNow = _spellLast = spell;
                _anim.SetInteger("spell", _spellBook.IndexOf(spell)); //активруем анимацию текущего спела
            }
        }
        public void SpellInterrupt() // прерывание предыдущей способности
        {
            CancelInvoke(); // прерываем окончание прошлой способности
            if (_spellLast != _frostJumpSpell) // если прердыдущая способность не прыжок, отменяем заморозку
            {
                _isFreezeRot = false;
                _anim.SetBool("spellCasting", false);
            }
            if (_spellNow != null)
            {
                _spellNow = null;
                _anim.SetInteger("spell", 0);
                if (_spellLast != null) _spellBook[_spellBook.IndexOf(_spellLast)].SetActive(false); // прерываем эффект прошлого заклинания
            }
        }
        public void SpellEnd() // окончание способности
        {
            _speed = _basicSpeed;
            _isFreezeRot = false;
            _anim.SetBool("groundSoon", false);
            _anim.SetBool("spellCasting", false);
            if (_spellLast == _frostJumpSpell) _spellLast = null;
            if (_spellLast == _rampageBuff) _spellLast = null;
            if (_spellNow != null)
            {
                _spellBook[_spellBook.IndexOf(_spellNow)].SetActive(false);
                _spellNow = null;
            }
            _anim.SetInteger("spell", 0);
        }
        public void Casting() // каст способности в нужный момент
        {
            if (_spellNow == _lightingStrikeSpell) // если способность с ударами молний, то выпускаем молнии во врагов через заданное время
            {
                _spellBook[_spellBook.IndexOf(_lightingStrikeSpell)].SetActive(true);
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
            if (_spellNow != _lightingStrikeSpell) return; // если способность не молнии, выходим
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
