using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Singing Bar Shit")]
    [SerializeField] private GameObject _singingBarObject;
    [SerializeField] private Slider _singingSlider;
    [SerializeField] private Image _singingSliderFiller;
    [SerializeField] private Color colr;
    private bool _isPlayerSinging;

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        _singingSlider.maxValue = 100;
        _singingSlider.value = 0;
        _singingBarObject.SetActive(false);
    }

    public void SingingBarState(bool state, PlayerSing tempPlayerSing)
    {
        if(state)
        {
            _isPlayerSinging = true;
            StartCoroutine(PlayerIsSinging(tempPlayerSing));
        }
        else if(!state)
        {
            _isPlayerSinging = false;
        }
        
        _singingBarObject.SetActive(state);
    }

    IEnumerator PlayerIsSinging(PlayerSing tempPlayerSing)
    {
        while (_isPlayerSinging)
        {
            _singingSlider.value = tempPlayerSing.SingingLevel;
            if(tempPlayerSing.CurrentPitchLevel == PitchLevel.LowPitch)
            {
                //Red
                _singingSliderFiller.color = new Color32(192,0,0,255);
            }
            else if(tempPlayerSing.CurrentPitchLevel == PitchLevel.MediumPitch)
            {
                //Yellow
                _singingSliderFiller.color = new Color32(212,238,0,255);
            }
            else if(tempPlayerSing.CurrentPitchLevel == PitchLevel.HighPitch)
            {
                //Green
                _singingSliderFiller.color = new Color32(0,238,3,255);
            }
            yield return null;
        }

        _singingSlider.value = 0;
    }
}
