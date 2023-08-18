using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemController : SingableObject
{
    private enum UnlockAbility
    {
        FirstJump, SecondJump, Dash, Reveal
    }
    [SerializeField] private UnlockAbility _abilityToUnlock;
    [SerializeField] private PitchLevel[] _pitchTarget;
    private bool _activated;
    private string _targetAbility;
    const string JUMP = "Jump";
    const string DASH = "Dash";

    [Header("Tutorial Variables")]
    [SerializeField] private TutorialManager.TutorialType _targetTutorial;
    [SerializeField] private Transform _startTrans;


    [Header("References")] 
    [SerializeField] private PitchReceiver _pitchReceiver;
    [SerializeField] private SpriteRenderer _sr;
    private PlayerManager _playerManager;

    private void Awake() 
    {
        SetUpPitchReciever();
        switch (_abilityToUnlock)
        {
            case UnlockAbility.FirstJump:
                _targetAbility = JUMP;
                break;
            case UnlockAbility.SecondJump:
                _targetAbility = JUMP;
                break;
            case UnlockAbility.Dash:
                _targetAbility = DASH;
                break;
            case UnlockAbility.Reveal:
                _targetAbility = null;
            break;
        }
    }   

    private void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
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
            if (_targetTutorial != TutorialManager.TutorialType.Null)
            {
                GameEvents.unlockTutorial?.Invoke(_targetTutorial, _startTrans);
            }
            if (_targetAbility != null)
            {
         
                if(_targetAbility == JUMP)
                {
                    _playerManager.CallPlayerExtra();

                }
                else if(_targetAbility == DASH)
                {
                    PlayerManager.PlayerTrans.GetComponent<PlayerController>()._dashUnlocked = true;
                }
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            _sr.color = Color.green;
            _activated = true;
            SoundManager.Instance.PlaySound3D(SoundManager.GameSoundType.TotemChime, transform.position);
        }
    }
}
