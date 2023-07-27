using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingSpiritBehaviour : SingableObject
{
    [Header("References")]
    [SerializeField] private Collider2D _col2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PitchReceiver _pitchReceiver;
    [SerializeField] private LayerMask _canGoSleepLayerMask;


    [Header("Actual Sleeping Spirit")]
    [SerializeField] private Transform _sleepingSpirit;


    [Header("Variables")]
    public PitchLevel[] _pitchTarget;
    [SerializeField] private float _moveSpeed;
    private Transform _playerPos;
    private Vector3 _movingTargetPos;
    private bool _alreadyMoving;

    //Coroutine TryingToDetectPitchCO;

    private void Awake() 
    {
        SetUpPitchReciever();
    }

    public override void SetUpPitchReciever()
    {
        if(_pitchReceiver == null)
        {
            foreach (Transform child in transform)
            {
                if(child.TryGetComponent<PitchReceiver>(out PitchReceiver pitchReceiver))
                {
                    _pitchReceiver = pitchReceiver;
                }
            }
        }
        _pitchReceiver.Init(PlayPitchBehaviour, _pitchTarget);
    }

    public override void PlayPitchBehaviour()
    {
        //Gets position sleeping spirit is going to
        float xPos = PlayerManager.PlayerTrans.position.x;
        _movingTargetPos = new Vector3(xPos, _sleepingSpirit.position.y);

        if(!_alreadyMoving)
        {
            _alreadyMoving = true;
            StartCoroutine(MoveToSingingSpot());
        }
    }

    void Start()
    {
        _playerPos = PlayerManager.PlayerTrans;
    }

    IEnumerator MoveToSingingSpot()
    {
        //Moving State
        _spriteRenderer.color = _spriteRenderer.color = new Color32(255,255,255,120);
        _col2D.enabled = false;

        bool _isPlayerStillOnMe = true;

        //Put into one while loop for the special case the player moves but not out of the sleeping spirit
        while (_sleepingSpirit.position != _movingTargetPos || _isPlayerStillOnMe)
        {
            //Moves sleeping spirit to player last sung position
            if(_sleepingSpirit.position != _movingTargetPos)
            {
                _sleepingSpirit.position = Vector3.MoveTowards(_sleepingSpirit.position, _movingTargetPos, _moveSpeed*Time.deltaTime);
            }
            else
            //Checks until player isn't directly in the sleeping spirit so it can become a block again
            {
                RaycastHit2D hit = Physics2D.BoxCast(_sleepingSpirit.position, _spriteRenderer.bounds.size, 0f, Vector2.zero, 0, _canGoSleepLayerMask);
                if(hit.collider == null)
                {
                    _isPlayerStillOnMe = false;
                }
            }
            yield return null;
        }


        //Sleeping State
        _spriteRenderer.color = new Color32(255,255,255,255);
        _col2D.enabled = true;
        _alreadyMoving = false;
    }
}
