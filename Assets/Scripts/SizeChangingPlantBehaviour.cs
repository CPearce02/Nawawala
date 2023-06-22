using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangingPlantBehaviour : MonoBehaviour
{
    public PitchLevel _pitchTarget;
    [SerializeField] private bool AmIExpanded;

    [Header("Animator")]
    [SerializeField] private Animator _anim;
    [SerializeField] private Collider2D _boxCol2D;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    const string EXPAND = "ShapeChangingPlant_Expand";
    const string SHRINK = "ShapeChangingPlant_Shrink";
    const string ALREXPAND = "ShapeChangingPlant_AlrExpand";
    const string ALRSHRINK = "ShapeChangingPlant_AlrShrink";

    Coroutine TryingToChangeStateCO;

    private void Start() 
    {
        if(AmIExpanded)
        {
            _boxCol2D.isTrigger = false;
            _anim.Play(ALREXPAND);
        }
        else if(!AmIExpanded)
        {
            _boxCol2D.isTrigger = true;
            _anim.Play(ALRSHRINK);
        }

        if(_pitchTarget == PitchLevel.LowPitch)
        {
            //Red
            _spriteRenderer.color = new Color32(192,0,0,255);
        }
        else if(_pitchTarget == PitchLevel.MediumPitch)
        {
            //Yellow
            _spriteRenderer.color = new Color32(212,238,0,255);
        }
        else if(_pitchTarget == PitchLevel.HighPitch)
        {
            //Green
            _spriteRenderer.color = new Color32(0,238,3,255);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.TryGetComponent<TempPlayerSing>(out TempPlayerSing tempPlayerSing))
        {
            Debug.Log(tempPlayerSing.IsSinging);
            if(tempPlayerSing.IsSinging)
            {
                StartCoroutine(ChangeMyState(tempPlayerSing));
            }
        }
    }

    IEnumerator ChangeMyState(TempPlayerSing tempPlayerSing)
    {
        float singToMeDuration = 0;
        while (singToMeDuration < 1f)
        {
            if(tempPlayerSing.CurrentPitchLevel == _pitchTarget)
            {
                singToMeDuration += Time.deltaTime;
            }
            else
            {
                if(singToMeDuration - Time.deltaTime > 0)
                {
                    singToMeDuration -= Time.deltaTime;
                }
                else
                {
                    singToMeDuration = 0;
                }
            }
            yield return null;
        }

        if(AmIExpanded)
        {
            _boxCol2D.isTrigger = true;
            _anim.Play(SHRINK);
        }
        else if(!AmIExpanded)
        {
            _boxCol2D.isTrigger = false;
            _anim.Play(EXPAND);
        }

        AmIExpanded = !AmIExpanded;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(TryingToChangeStateCO != null)
        {
            StopCoroutine(TryingToChangeStateCO);
        }
    }
}
