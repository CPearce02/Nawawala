using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchSymbolBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 _onePitchPos;
    [SerializeField] private Vector3[] _twoPitchPos;
    [SerializeField] private Vector3[] _threePitchPos;

    [Header("Pitch Objects")]
    [SerializeField] private GameObject _lowPitchObj;
    [SerializeField] private GameObject _mediumPitchObj;
    [SerializeField] private GameObject _highPitchObj;

    public void Init(PitchLevel[] pitchs)
    {
        _lowPitchObj.SetActive(false);
        _mediumPitchObj.SetActive(false);
        _highPitchObj.SetActive(false);

        if(pitchs.Length == 1)
        {
            switch (pitchs[0])
            {
                case PitchLevel.LowPitch: 
                    _lowPitchObj.transform.localPosition = _onePitchPos;
                    _lowPitchObj.SetActive(true);
                    break;
                case PitchLevel.MediumPitch: 
                    _mediumPitchObj.transform.localPosition = _onePitchPos;
                    _mediumPitchObj.SetActive(true);
                    break;
                case PitchLevel.HighPitch: 
                    _highPitchObj.transform.localPosition = _onePitchPos;
                    _highPitchObj.SetActive(true);
                    break;
            }
        }
        else if(pitchs.Length == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                switch (pitchs[i])
                {
                    case PitchLevel.LowPitch: 
                        _lowPitchObj.transform.localPosition = _twoPitchPos[i];
                        _lowPitchObj.SetActive(true);
                        break;
                    case PitchLevel.MediumPitch: 
                        _mediumPitchObj.transform.localPosition = _twoPitchPos[i];
                        _mediumPitchObj.SetActive(true);
                        break;
                    case PitchLevel.HighPitch: 
                        _highPitchObj.transform.localPosition = _twoPitchPos[i];
                        _highPitchObj.SetActive(true);
                        break;
                }
            }
        }
        else if(pitchs.Length == 3)
        {
            _lowPitchObj.transform.localPosition = _threePitchPos[0];
            _lowPitchObj.SetActive(true);
            _mediumPitchObj.transform.localPosition = _threePitchPos[1];
            _mediumPitchObj.SetActive(true);
            _highPitchObj.transform.localPosition = _threePitchPos[2];
            _highPitchObj.SetActive(true);
        }
        else
        {
            Debug.LogError("Has more than 3 pitches: " + transform.parent.name);
        }
    }

    
}
