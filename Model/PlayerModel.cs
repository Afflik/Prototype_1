using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerModel : PlayerSettings
    {
        #region Control
        public void Roll()
        {
            if (_attackType != 0) return;
            if (_spellNow != null) return;

            if (_move != 0)
            {
                _anim.SetInteger("roll", _move);
                _rig.velocity = _move * 10 * _trfm.forward;
            }
            else
            {
                _anim.SetInteger("roll", -1);
                _rig.velocity = -1 * 10 * _trfm.forward;
            }
            _isRoll = true;
        }
        public void RollEnd()
        {
            _rig.velocity = Vector3.zero;
            _anim.SetInteger("roll", 0);
            _isRoll = false;
        }
        public void Moving(float speed) // передвижение
        {
            if (_attackType != 0 || _isFreezeRot || _isRoll) return;

            if (Input.GetAxis("Vertical") >= 0.2f)
            {
                _anim.SetInteger("move", 1);
            }
            else if (Input.GetAxis("Vertical") <= -0.2f)
            {
                speed /= 2;
                _anim.SetInteger("move", -1);
            }
            else
            {
                _anim.SetInteger("move", 0);
            }
            _move = _anim.GetInteger("move");
            if (Input.GetAxis("Vertical") < 0.2f && Input.GetAxis("Vertical") > 0) speed = 0;
            else if (Input.GetAxis("Vertical") != 0)

                _trfm.position += Input.GetAxis("Vertical") * (_trfm.forward * Time.deltaTime * speed);
        }
        public void PlayerLook()
        {
            if (!_isFreezeRot)
            {
                PlayerLook(_trfm, _camera.ScreenPointToRay(Input.mousePosition));
            }
        }
        public void GroundDistance()
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
        protected void MiniCharge()
        {
            _rig.velocity = transform.forward * 5;
        }
        protected void AddForce()
        {
            _rig.velocity = ((transform.forward * 5) + (transform.up * 5));
        }
        #endregion

        #region Attack
        public void Attack(int num)
        {
            if (_isRoll) return;
            if (_anim.GetInteger("spell") != 2)
            {
                SpellInterrupt();
                if (num == 1 && _attackType != 2)
                {
                    if (_comboLeftClick == 1) _comboLeftClick = 2;
                    _anim.SetInteger("comboLC", _comboLeftClick);
                    _anim.SetInteger("attack", num);
                    _attackType = num;
                }
                else if (num == 2)
                {
                    if (_comboRightClick == 1) _comboRightClick = 2;
                    _anim.SetInteger("comboRC", _comboRightClick);
                    _anim.SetInteger("attack", num);
                    _attackType = num;
                }
            }
        }
        protected void AttackEffect()
        {
            GameObject _attEff = null;
            if (_spellNow == _frostJumpSpell)
            {
                _attEff = Instantiate(_spellBook[_spellBook.IndexOf(_frostJumpSpell)],
                                      _spellBook[_spellBook.IndexOf(_frostJumpSpell)].transform.position,
                                      _spellBook[_spellBook.IndexOf(_frostJumpSpell)].transform.rotation);
                Destroy(_attEff, 2.5f);
            }
            else if (_attackType == 2 || _isBuffOn)
            {
                _attEff = Instantiate(_effects[0], _effects[0].transform.position, _effects[0].transform.rotation, _effects[0].transform.parent);
                _attEff.SetActive(true);
                Destroy(_attEff, 1f);
            }
            if (_attEff != null) _attEff.SetActive(true);
        }
        protected void AttackStart()
        {
            if (_isBuffOn && _attackType == 1)
            {
                AttackEffect();
            }
        }
        protected void AttackEnd()
        {
            _attackType = 0;
            _anim.SetInteger("attack", _attackType);
            _comboLeftClick = _comboRightClick = 0;
            _anim.SetInteger("comboLC", _comboLeftClick);
            _anim.SetInteger("comboRC", _comboRightClick);
        }
        protected void ComboReset()
        {
            _comboLeftClick = _comboRightClick = 1;
            _anim.SetInteger("comboLC", _comboLeftClick);
            _anim.SetInteger("comboRC", _comboRightClick);
        }
        #endregion

        #region Spells
        public void Spell()
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
            if (_isBuffOn)
            {
                if (Timer(_buffTime)) Buff();
                if (_trfm.localScale.y < _buffScale.y)
                {
                    _trfm.localScale += _scaleUpStep;
                }
                _spellBook[4].SetActive(true);
                if (_spellNow != _hurricaneSpell) _speed = 5;
            }
            if (!_isBuffOn)
            {
                if (_trfm.localScale.y > _defaultScale.y)
                {
                    _trfm.localScale -= _scaleUpStep;
                    _spellBook[4].SetActive(false);
                }
            }
        }
        public void SpellCast(GameObject spell, float time)
        {
            if (_isRoll) return;
            if (_spellLast != _frostJumpSpell && _attackType == 0)
            {
                if (_spellLast == _rampageBuff) return;
                if (spell == _rampageBuff && !_isBuffOn)
                {
                    SpellInterrupt();
                    _buffTime = time;
                    _isFreezeRot = true;
                    _spellLast = spell;
                    _spellNow = spell;
                    _anim.SetInteger("spell", _spellBook.IndexOf(spell));
                    return;
                }
                if (_spellLast != _rampageBuff) SpellInterrupt();

                if (spell == _hurricaneSpell)
                {
                    spell.SetActive(true);
                }

                if (spell != _frostJumpSpell)
                {
                    Invoke("SpellEnd", time);
                }
                if (spell != _rampageBuff) _spellNow = spell;
                _spellLast = _spellNow;
                _anim.SetInteger("spell", _spellBook.IndexOf(spell));
            }
        }
        public void SpellInterrupt()
        {
            CancelInvoke();
            if (_spellLast != _frostJumpSpell)
            {
                _isFreezeRot = false;
                _anim.SetBool("spellCasting", false);
            }
            if (_spellNow != null)
            {
                _spellNow = null;
                _anim.SetInteger("spell", 0);
                if (_spellLast != null) _spellBook[_spellBook.IndexOf(_spellLast)].SetActive(false);
            }
        }
        public void SpellEnd()
        {
            _speed = _basicSpeed;
            _isFreezeRot = false;
            _anim.SetBool("groundSoon", false);
            _anim.SetBool("spellCasting", false);
            if (_spellLast == _frostJumpSpell)
            {
                _spellLast = null;
            }
            if (_spellNow != null)
            {
                _spellBook[_spellBook.IndexOf(_spellNow)].SetActive(false);
                _spellNow = null;
            }
            if (_spellLast == _rampageBuff) _spellLast = null;
            _anim.SetInteger("spell", 0);
        }
        public void Casting()
        {
            if (_spellNow == _lightingStrikeSpell)
            {
                _spellBook[_spellBook.IndexOf(_lightingStrikeSpell)].SetActive(true);
                Invoke("Lightings", 1);
            }
            _anim.SetBool("spellCasting", true);
        }
        public void Buff()
        {
            if (_isBuffOn) _isBuffOn = false;
            else
            {
                _isBuffOn = true;
                _anim.SetInteger("spell", 0);
            }
        }

        private void Lightings()
        {
            if (_spellNow != _lightingStrikeSpell) return;
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 10, Quaternion.identity, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < hitColliders.Length; i++)
            {
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
