using System;
using Interfaces;
using UnityEngine;

namespace Enemies.EnemyStates
{
    public class ChaseState : IState
    {
        private Vector3 _lastKnownPosition;
        private EnemyStateMachine _enemy;
        private float _waitTime = 3;
        private bool _inPosition;
        private bool _attacked;

        public ChaseState(Vector3 position)
        {
            this._lastKnownPosition = position;
        }

        public void Enter(EnemyStateMachine enemy)
        {
            this._enemy = enemy;

        }

        public void Execute(EnemyStateMachine enemy)
        {
            if (_enemy.player.IsSinging)
            {
                _enemy.ChangeState(new ChaseState(_enemy.player.transform.position));
            }

            if (_attacked)
            {
                _enemy.ChangeState(new IdleState());
            }

            GoToPosition();
            if (!_inPosition) return;

            //Once in position - wait
            if (_waitTime > 0)
            {
                _waitTime -= Time.deltaTime;
                var hit = Physics2D.OverlapCircle(_enemy.transform.position, 8);
                if (hit != null && hit.TryGetComponent(out PlayerManager player))
                {
                    if (player.collectedSouls != null && !_attacked)
                    {
                        //player.DisperseSouls();
                        _attacked = true;
                    }
                }
            }
            else 
            {
                _enemy.ChangeState(new IdleState());
            }

            
        }

        private void GoToPosition() 
        {
            _enemy.transform.position = Vector2.MoveTowards(_enemy.transform.position, _lastKnownPosition, _enemy.enemyData.moveSpeed * 2 * Time.deltaTime);
            if(_enemy.transform.position == _lastKnownPosition)
            {
                _inPosition = true;
            }
        }

        public void Exit()
        {

        }

    }
}