using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPitchSymbolHandler : MonoBehaviour
{
    [Header("Pitch Objects")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _lowPitchSprite;
    [SerializeField] private Sprite _mediumPitchSprite;
    [SerializeField] private Sprite _highPitchSprite;

    void Start()
    {
        StopShowingSymbol();
    }

    public void UpdatePlayerPitchSymbol(PitchLevel pitchLevel)
    {
        switch (pitchLevel)
        {
            case PitchLevel.LowPitch:
                _spriteRenderer.sprite = _lowPitchSprite;
                break;
            case PitchLevel.MediumPitch:
                _spriteRenderer.sprite = _mediumPitchSprite;
                break;
            case PitchLevel.HighPitch:
                _spriteRenderer.sprite = _highPitchSprite;
                break;
        }
    }

    public void StopShowingSymbol()
    {
        _spriteRenderer.sprite = null;
    }
}
