using Interfaces;
using System;
using UnityEngine;

namespace Enemies.EnemyStates
{
    public class IdleState : IState
    {
        private EnemyStateMachine _enemy;
        private Vector3 _closestPosition;
        private bool _inPosition;
        private bool _inStartingPosition;
        private float _closestDistance = Mathf.Infinity;
        private int _currentIndex; 
        public void Enter(EnemyStateMachine enemy)
        {
            this._enemy = enemy;
        }

        public void Execute(EnemyStateMachine enemy)
        {
            //var hit = Physics2D.OverlapCircle(_enemy.transform.position, 50);
            //if (hit != null && hit.TryGetComponent(out TempPlayerSing player))
            //{
            //    if (player.IsSinging)
            //    {
            //        _enemy.ChangeState(new ChaseState(player.transform.position));
            //    }
            //}

            if (_enemy.player.IsSinging) 
            {
                _enemy.ChangeState(new ChaseState(_enemy.player.transform.position));
            }

            //Get into position
            FindAndMoveIntoPosition();
            if (!_inStartingPosition) return;

            MoveToNextPosition();
            
        }

        public void Exit()
        {
            
        }

        private void FindAndMoveIntoPosition()
        {
            if (_inStartingPosition) return;
            //Find closest Postion
            foreach (Transform position in _enemy.positions)
            {
                float distance = Vector3.Distance(_enemy.transform.position, position.transform.position);
                if (distance < _closestDistance)
                {
                    _closestDistance = distance;
                    _closestPosition = position.position;
                    _currentIndex = Array.IndexOf(_enemy.positions, position);
                    _currentIndex = (_currentIndex + 1) % _enemy.positions.Length;
                }
            }
            //Move to it

            _enemy.transform.position = Vector2.MoveTowards(_enemy.transform.position, _closestPosition, _enemy.enemyData.moveSpeed * 2 * Time.deltaTime);

            if(_enemy.transform.position == _closestPosition) 
            {
                _inStartingPosition = true;
            }
        }

        private void MoveToNextPosition() 
        {
            if (!_inPosition) 
            {
                if (_enemy.transform.position == _enemy.positions[_currentIndex].position)
                {
                    _inPosition = true;
                }
                _enemy.transform.position = Vector2.MoveTowards(_enemy.transform.position, _enemy.positions[_currentIndex].position, _enemy.enemyData.moveSpeed * 2 * Time.deltaTime);
            }
            else 
            {
                _currentIndex = (_currentIndex + 1) % _enemy.positions.Length;
                Debug.Log(_enemy.positions[_currentIndex]);
                _inPosition = false;
            }
        }
    }
}