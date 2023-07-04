using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PitchReceiver : MonoBehaviour
{
    Action myFunction;
    //[Tooltip("DONT ASSIGN IN EDITOR")]
    private PitchLevel _targetPitchLevel;
    public void Init(Action givenFunction, PitchLevel pitchLevelWanted)
    {
        myFunction = givenFunction;
        _targetPitchLevel = pitchLevelWanted;
    }

    public void PitchCall(PitchLevel callingPitch)
    {
        if(callingPitch == _targetPitchLevel)
        {
            myFunction();
        }
    }
}
