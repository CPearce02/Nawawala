using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemController : SingableObject
{
    private enum UnlockAbility
    {
        Jump, Dash
    }

    [SerializeField] private UnlockAbility _abilityToUnlock;
    [SerializeField] private PitchLevel _pitchTarget;
    private bool _activated;

    private string _targetAbility;
    const string JUMP = "Jump";
    const string DASH = "Dash";

    [Header("References")] 
    [SerializeField] private PitchReceiver _pitchReceiver;
    private SpriteRenderer _sr;

    private void Awake() 
    {
        SetUpPitchReciever();
        switch (_abilityToUnlock)
        {
            case UnlockAbility.Jump:
                _targetAbility = JUMP;
                break;
            case UnlockAbility.Dash:
                _targetAbility = DASH;
                break;
        }

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
        if(!_activated)
        {
            GameEvents.onAbilityLock(_targetAbility, false);
            _sr.color = Color.green;
            _activated = true;
        }
    }

    void Start()
    {
        if(_sr == null)
        {
            _sr = GetComponent<SpriteRenderer>();
        }
    }


    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (_activated) return;
    //     if (collision.TryGetComponent<TempPlayerSing>(out TempPlayerSing tempPlayerSing))
    //     {
    //         if (tempPlayerSing.IsSinging)
    //         {
    //             if (tempPlayerSing.CurrentPitchLevel == _pitchTarget)
    //             {
                    // GameEvents.onAbilityLock(_targetAbility, false);
                    // _sr.color = Color.green;
                    // _activated = true;
    //             }
    //         }
    //     }
    // }
}
