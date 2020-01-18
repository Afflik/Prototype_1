using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerController: IOnUpdate
    {
        private PlayerModel _player = Object.FindObjectOfType<PlayerModel>();
        private SpellManager.Spell[] _spells;

        private KeyCode _potion = KeyCode.Q;
        private KeyCode _lockLook = KeyCode.LeftShift;
        private KeyCode _one = KeyCode.Alpha1;
        private KeyCode _two = KeyCode.Alpha2;
        private KeyCode _tree = KeyCode.Alpha3;
        private KeyCode _four = KeyCode.Alpha4;

        private int _leftClick = 0;
        private int _rightClick = 1;

        public void Init()
        {
            _spells = _player.SpellBook;
        }

        public void OnUpdate()
        {
            if (_player.IsStunned) return;
            if (_player.IsDeath) return;

            if (Input.GetMouseButtonDown(_leftClick))
            {
                _player.MainAttack(1);
            }
            if (Input.GetMouseButtonDown(_rightClick))
            {
                _player.MainAttack(2);
            }

            if (Input.GetKeyDown(_lockLook))
            {
                _player.LockLook(true);
            }
            if (Input.GetKeyUp(_lockLook))
            {
                _player.LockLook(false);
            }

            if (Input.GetKeyDown(_potion)) // первый спелл
            {
                _player.UsePotion();
            }

            if (Input.GetKeyDown(_one)) // первый спелл
            {
                _player.SpellCast(0);
            }
            if (Input.GetKeyDown(_two)) // второй спелл
            {
                _player.SpellCast(1);
            }
            if (Input.GetKeyDown(_tree)) // третий спелл
            {
                _player.SpellCast(2);
            }
            if (Input.GetKeyDown(_four)) // четвертый спелл
            {
                _player.SpellCast(3);
            }
            _player.Moving(_player.Speed); // передвижение
            _player.PlayerLook(); // слежение за мышкой
            _player.GroundDistance(); // проверка дистанции до земли
            _player.Spell(); // состояние способностей
        }
    }
}
