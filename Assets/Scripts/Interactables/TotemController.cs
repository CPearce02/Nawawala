using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemController : MonoBehaviour
{
    [SerializeField] private string _abilityType;
    public PitchLevel _pitchTarget;
    private bool _activated;

    private SpriteRenderer _sr;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_activated) return;
        if (collision.TryGetComponent<TempPlayerSing>(out TempPlayerSing tempPlayerSing))
        {
            if (tempPlayerSing.IsSinging)
            {
                if (tempPlayerSing.CurrentPitchLevel == _pitchTarget)
                {
                    GameEvents.onAbilityLock(_abilityType, false);
                    _sr.color = Color.green;
                    _activated = true;
                }
            }
        }
    }

}
