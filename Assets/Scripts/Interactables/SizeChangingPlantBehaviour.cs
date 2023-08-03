using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangingPlantBehaviour : SingableObject
{
    public PitchLevel[] _pitchTarget;
    [SerializeField] private bool AmIExpanded;


    [Header("References")]
    [SerializeField] private Collider2D _boxCol2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PitchReceiver _pitchReceiver;


    [Header("Animator")]
    [SerializeField] private Animator _anim;
    const string EXPAND = "ShapeChangingPlant_Expand";
    const string SHRINK = "ShapeChangingPlant_Shrink";
    const string ALREXPAND = "ShapeChangingPlant_AlrExpand";
    const string ALRSHRINK = "ShapeChangingPlant_AlrShrink";

    private void Awake() 
    {
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
        ChangeSize();
    }

    private void Start() 
    {
        if(AmIExpanded)
        {
            _boxCol2D.enabled = true;
            _anim.Play(ALREXPAND);
        }
        else if(!AmIExpanded)
        {
            _boxCol2D.enabled = false;
            _anim.Play(ALRSHRINK);
        }

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
    }

    private void ChangeSize()
    {
        if(AmIExpanded)
        {
            SoundManager.Instance.PlaySound3D(SoundManager.GameSoundType.GrowingPlantSFX, transform.position);
            _boxCol2D.enabled = false;
            _anim.Play(SHRINK);
        }
        else if(!AmIExpanded)
        {
            SoundManager.Instance.PlaySound3D(SoundManager.GameSoundType.GrowingPlantSFX, transform.position);
            _boxCol2D.enabled = true;
            _anim.Play(EXPAND);
        }

        AmIExpanded = !AmIExpanded;
    }
}
