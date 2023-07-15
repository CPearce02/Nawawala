using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingSpiritBehaviour : SingableObject
{
    [Header("References")]
    [SerializeField] private Collider2D _col2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PitchReceiver _pitchReceiver;

    [Header("Variables")]
    public PitchLevel[] _pitchTarget;
    [SerializeField] private float _moveSpeed;

    private Transform _playerPos;

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
        StartCoroutine(MoveToSingingSpot());
    }

    void Start()
    {
        // if(_pitchTarget == PitchLevel.LowPitch)
        // {
        //     //Red
        //     _spriteRenderer.color = new Color32(192,0,0,255);
        // }
        // else if(_pitchTarget == PitchLevel.MediumPitch)
        // {
        //     //Yellow
        //     _spriteRenderer.color = new Color32(212,238,0,255);
        // }
        // else if(_pitchTarget == PitchLevel.HighPitch)
        // {
        //     //Green
        //     _spriteRenderer.color = new Color32(0,238,3,255);
        // }

        _playerPos = PlayerManager.PlayerTrans;
    }

    IEnumerator MoveToSingingSpot()
    {
        _col2D.enabled = false;
        float xPos = PlayerManager.PlayerTrans.position.x;
        Vector3 targetPos = new Vector3(xPos, transform.position.y);

        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed*Time.deltaTime);
            yield return null;
        }

        _col2D.enabled = true;
    }



    // private void OnTriggerEnter2D(Collider2D other) 
    // {
    //     if(other.TryGetComponent<TempPlayerSing>(out TempPlayerSing tempPlayerSing))
    //     {
    //         if(tempPlayerSing.IsSinging)
    //         {
    //             TryingToDetectPitchCO = StartCoroutine(DetectRightPitch(tempPlayerSing));
    //         }
    //     }
    // }

    // IEnumerator DetectRightPitch(TempPlayerSing tempPlayerSing)
    // {
    //     while (tempPlayerSing.CurrentPitchLevel != _pitchTarget)
    //     {
    //         yield return null;
    //     }

    //     StartCoroutine(MoveToSingingSpot(tempPlayerSing));
    // }



    // private void OnTriggerExit2D(Collider2D other) 
    // {
    //     if(TryingToDetectPitchCO != null)
    //     {
    //         StopCoroutine(TryingToDetectPitchCO);
    //     }
    // }


}
