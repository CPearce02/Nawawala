using Enemies;

namespace Interfaces
{
    public interface IState
    {
        void Enter(EnemyStateMachine enemyStateMachine);
        void Execute(EnemyStateMachine enemyStateMachine);
        void Exit();
    }
}