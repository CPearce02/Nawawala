using Interfaces;
using Unity.Collections;
using UnityEngine;
using Enemies.EnemyStates;

namespace Enemies
{
    public class EnemyStateMachine : SingableObject
    {
        protected IState CurrentState { get; set; }
        [SerializeField][ReadOnly] string stateName;


        [Header("Pitch Stuff")]
        [SerializeField] private PitchReceiver _pitchReceiver;
        [SerializeField] private PitchLevel _pitchTarget;
        

        [Header("Settings")]
        [SerializeField] public EnemyData enemyData;
        public Animator animator;
        [field:SerializeField] public Rigidbody2D Rb { get; private set; }
        [field:SerializeField] public LayerMask PlayerLayer { get; private set; }

        public Transform[] positions;

        public TempPlayerSing player;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            SetUpPitchReciever();
        }

        public override void SetUpPitchReciever()
        {
            if(_pitchReceiver == null)
            {
                _pitchReceiver.GetComponent<PitchReceiver>();
            }
            _pitchReceiver.Init(PlayPitchBehaviour, _pitchTarget);
        }

        public override void PlayPitchBehaviour()
        {
            //Play whatever the pitch is meant to activate 
        }
        
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent<PlayerManager>(out PlayerManager pm)) 
            {
                if(pm.collectedSouls.Count > 0) 
                {
                    pm.DisperseSouls();
                }
            }
        }
    }
}
