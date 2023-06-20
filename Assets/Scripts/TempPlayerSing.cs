using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PitchLevel
{
    LowPitch, MediumPitch, HighPitch
}

public class TempPlayerSing : MonoBehaviour
{
    public bool IsSinging;
    public float SingingLevel{get{return _singingLevel;}}
    public PitchLevel CurrentPitchLevel;
    [SerializeField] private float _singingLevel;
    [SerializeField] private float _singingStrength;
    private const float MaxSingingLevel = 100f;
    [SerializeField] private CircleCollider2D _singingDetection;
    Coroutine delayCo;

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            IsSinging = true;
            _singingDetection.enabled = true;
            UiManager.Instance.SingingBarState(true, this);
            if(delayCo != null)
            {
                StopCoroutine(delayCo);
            }
        }
        
        if(Input.GetMouseButton(1))
        {
            if(_singingLevel + Time.deltaTime*_singingStrength <= MaxSingingLevel)
            {
                _singingLevel += Time.deltaTime*_singingStrength;
            }
            else
            {
                _singingLevel = MaxSingingLevel;
            }

            if(SingingLevel < 33)
            {
                CurrentPitchLevel = PitchLevel.LowPitch;
            }
            else if(SingingLevel < 66)
            {
                CurrentPitchLevel = PitchLevel.MediumPitch;
            }
            else if(SingingLevel <= 100)
            {
                CurrentPitchLevel = PitchLevel.HighPitch;
            }
        }
        else
        {
            if(_singingLevel - Time.deltaTime*_singingStrength > 0)
            {
                _singingLevel -= Time.deltaTime*_singingStrength;
            }
            else
            {
                if(delayCo != null)
                {
                    StopCoroutine(delayCo);
                }
                TurnOffSinging();
            }
        }
        
        if(Input.GetMouseButtonUp(1))
        {
            delayCo = StartCoroutine(DelayTurnOffSinging());
        }
    }

    IEnumerator DelayTurnOffSinging()
    {
        yield return new WaitForSeconds(1f);
        TurnOffSinging();
    }

    private void TurnOffSinging()
    {
        IsSinging = false;
        _singingDetection.enabled = false;
        UiManager.Instance.SingingBarState(false, this);
        _singingLevel = 0;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        //Debug.Log("Champ");
    }
}
