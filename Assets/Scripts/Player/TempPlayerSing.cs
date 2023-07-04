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

    private bool _isSinging;
    private bool _reachedMaxPitch;
    private const float MaxSingingLevel = 100f;

    [Header("Singing")]
    [SerializeField] private float _singDistance;
    [SerializeField] private LayerMask _singingLayerMask;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            IsSinging = true;
            _isSinging = true;
            UiManager.Instance.SingingBarState(true, this);
        }

        if(_isSinging)
        {
            if(_singingLevel + Time.deltaTime*_singingStrength <= MaxSingingLevel)
            {
                _singingLevel += Time.deltaTime*_singingStrength;
            }
            else
            {
                _singingLevel = MaxSingingLevel;
                _reachedMaxPitch = true;
            }

            if(SingingLevel < 34)
            {
                CurrentPitchLevel = PitchLevel.LowPitch;
            }
            else if(SingingLevel < 67)
            {
                CurrentPitchLevel = PitchLevel.MediumPitch;
            }
            else if(SingingLevel < 100)
            {
                CurrentPitchLevel = PitchLevel.HighPitch;
                
            }
        }

        if(Input.GetMouseButtonUp(1) || _reachedMaxPitch)
        {
            _reachedMaxPitch = false;

            _isSinging = false;

            RaycastHit2D[] raycasthits = Physics2D.CircleCastAll(transform.position, _singDistance, Vector2.zero, 0f, _singingLayerMask);

            foreach (var hit in raycasthits)
            {
                if(hit.transform.TryGetComponent<PitchReceiver>(out PitchReceiver pitchReceiver))
                {
                    pitchReceiver.PitchCall(CurrentPitchLevel);
                }
            }

            TurnOffSinging();
        }
    }


    private void TurnOffSinging()
    {
        UiManager.Instance.SingingBarState(false, this);
        _singingLevel = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, _singDistance);
    }
}
