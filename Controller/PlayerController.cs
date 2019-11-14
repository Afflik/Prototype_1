using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerController: IOnUpdate, Init
    {
        private PlayerModel _player = Object.FindObjectOfType<PlayerModel>();
        private SpellManager.Spell[] _spells;

        private KeyCode _roll = KeyCode.LeftShift;
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
            if (Input.GetMouseButtonDown(_leftClick))
            {
                _player.Attack(1);
            }
            if (Input.GetMouseButtonDown(_rightClick))
            {
                _player.Attack(2);
            }

            if (Input.GetKeyDown(_roll))
            {
                _player.Roll();
            }

            if (Input.GetKeyDown(_one)) // первый спелл
            {
                _player.SpellCast(0, _spells[0].time);
            }
            if (Input.GetKeyDown(_two)) // второй спелл
            {
                _player.SpellCast(1, _spells[1].time);
            }
            if (Input.GetKeyDown(_tree)) // третий спелл
            {
                _player.SpellCast(2, _spells[2].time);
            }
            if (Input.GetKeyDown(_four)) // четвертый спелл
            {
                _player.SpellCast(3, _spells[3].time);
            }
            _player.Moving(_player.Speed); // передвижение
            _player.PlayerLook(); // слежение за мышкой
            _player.GroundDistance(); // проверка дистанции до земли
            _player.Spell(); // состояние способностей
        }
    }
}
