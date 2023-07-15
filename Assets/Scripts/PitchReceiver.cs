using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PitchReceiver : MonoBehaviour
{
    Action myFunction;
    //[Tooltip("DONT ASSIGN IN EDITOR")]
    private PitchLevel[] _targetPitchLevel;
    [SerializeField] private PitchSymbolBehaviour _pitchSymbolBehaviour;
    public void Init(Action givenFunction, PitchLevel[] pitchLevelWanted)
    {
        myFunction = givenFunction;
        _targetPitchLevel = pitchLevelWanted;

        if(_pitchSymbolBehaviour != null)
        {
            _pitchSymbolBehaviour.Init(pitchLevelWanted);
        }
        // try
        // {
            
        // }
        // catch (System.Exception)
        // {
        //     Debug.LogError("No Pitch Symbol: " + transform.parent.name);
        //     throw;
        // }
        
    }

    public void PitchCall(PitchLevel callingPitch)
    {
        foreach (PitchLevel pitch in _targetPitchLevel)
        {
            if(callingPitch == pitch)
            {
                myFunction();
            }
        } 
    }
}

