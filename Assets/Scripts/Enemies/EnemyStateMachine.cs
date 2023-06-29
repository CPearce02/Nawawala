using Interfaces;
using Unity.Collections;
using UnityEngine;
using Enemies.EnemyStates;

namespace Enemies
{
    public class EnemyStateMachine : MonoBehaviour
    {
        protected IState CurrentState { get; set; }
        [SerializeField][ReadOnly] string stateName;
        
        [Header("Settings")]
        [SerializeField] public EnemyData enemyData;
        public Animator animator;
        [field:SerializeField] public Rigidbody2D Rb { get; private set; }
        [field:SerializeField] public LayerMask PlayerLayer { get; private set; }

        public Transform[] positions;

        public TempPlayerSing player;

        private void Awake() => Rb = GetComponent<Rigidbody2D>();

        
        public virtual void Start()
        {
            ChangeState(new IdleState());
        }
    
        public virtual void Update()
        {
            CurrentState.Execute(this);   
            stateName = CurrentState.ToString();
        }

        public void ChangeState(IState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = newState;
            CurrentState.Enter(this);
        }


    }
}
