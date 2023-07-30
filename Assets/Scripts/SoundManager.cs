using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SoundManager : MonoBehaviour
{
    public enum GameSoundType
    {
        PlayerStep, PlayerJump, PlayerDash, TotemChime, GrowingPlantSFX, ShrinkingPlantSFX, SpinningFlowerSFX, SleepingSpiritWakeUp, SleepingSpiritSleepAgain, SleepingSpiritWalk
    }

    public enum AudioMixerGroups
    {
        
    }

    public enum UISound
    {
        UIClickEnter, UIClickExit
    }

    public static SoundManager Instance;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource; //For 2D Sounds
    

    [Header("Sounds")]
    public GameSoundAudioClip[] GameSounds;
    public UISoundAudioClip[] UISounds;
    private Dictionary<GameSoundType, AudioClip> _gameSounds;
    private Dictionary<UISound, AudioClip> _uISounds;
    private static Dictionary<GameSoundType, float> soundTimerDictionary;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;


    [Header("Other")]
    [SerializeField] private GameObject oneShotUIObject; //UI Sounds
    [SerializeField] private Sound _soundObject; //For 3D Sounds
    [SerializeField] private Transform _soundObjPoolContainer;
    private ObjectPool<Sound> _soundsObjPool;
    Dictionary<object, float> _repeatingSoundsDic = new Dictionary<object, float>();
    private float _EMPTYFLOAT = 0f;

    private void Awake() 
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
    }

    private void Init()
    {
        _gameSounds = new Dictionary<GameSoundType, AudioClip>();
        foreach (var soundSet in GameSounds)
        {
            _gameSounds.Add(soundSet.SoundType, soundSet.audioClip);
        }

        _uISounds = new Dictionary<UISound, AudioClip>();
        foreach (var soundSet in UISounds)
        {
            _uISounds.Add(soundSet.SoundType, soundSet.audioClip);
        }
        
        soundTimerDictionary = new Dictionary<GameSoundType, float>();

        if(oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("Sound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }

        _soundsObjPool = new ObjectPool<Sound>(() => 
        {
            return Instantiate(_soundObject, transform.position, Quaternion.identity, _soundObjPoolContainer);
        }, soundObject => {
            soundObject.gameObject.SetActive(true);
        }, soundObject => {
            soundObject.gameObject.SetActive(false);
        }, soundObject => {
            Destroy(soundObject);
        }, true, 25, 30);

        PreLoad();
    }

    private void PreLoad()
    {
        List<Sound> piecesHandlers = new List<Sound>();
        for (int i = 0; i < 25; i++)
        {
            piecesHandlers.Add(_soundsObjPool.Get()); 
        }

        for (int i = 0; i < 25; i++)
        {
            _soundsObjPool.Release(piecesHandlers[i]);
        }
    }

    public void PlaySound2D(GameSoundType sound)
    {
        if(!SettingsData.SoundMuted)
        {
            oneShotAudioSource.PlayOneShot(_gameSounds[sound]);
        }
    }

    #region Play 3D Sound
    public void PlaySound3D(GameSoundType gameSound, Vector3 targetPosition)
    {
        if(!SettingsData.SoundMuted)
        {
            Sound audioObject = _soundsObjPool.Get();
            audioObject.transform.position = targetPosition;

            audioObject.PlaySound(_gameSounds[gameSound]);
        }
    }

    public void PlaySound3D(GameSoundType gameSound, Vector3 targetPosition, AudioMixerGroups audioMixerGroup)
    {
        if(!SettingsData.SoundMuted)
        {
            Sound audioObject = _soundsObjPool.Get();
            audioObject.transform.position = targetPosition;

            audioObject.PlaySound(_gameSounds[gameSound]);
        }
    }

    public void Release3DSound(Sound releaseTarget)
    {
        _soundsObjPool.Release(releaseTarget);
    }
    #endregion
    
    #region Play Repeating Sound
    public void PlayRepeatingSound(GameSoundType gameSound, object reference, float timeBetween)
    {
        if(!SettingsData.SoundMuted)
        {
            bool foundReference = false;

            for (int i = 0; i < _repeatingSoundsDic.Count; i++)
            {
                if(_repeatingSoundsDic.TryGetValue(reference, out _EMPTYFLOAT))
                {
                    foundReference = true;
                    break;
                }
            }
            
            if(!foundReference)
            {
                _repeatingSoundsDic.Add(reference, Time.time);
            }

            if(CanPlaySound(reference, timeBetween))
            {
                oneShotAudioSource.PlayOneShot(_gameSounds[gameSound]);
            }
        }
    }

    public void PlayRepeatingSound(GameSoundType gameSound, object reference, float timeBetween, Vector3 targetPosition)
    {
        if(!SettingsData.SoundMuted)
        {
            bool foundReference = false;

            for (int i = 0; i < _repeatingSoundsDic.Count; i++)
            {
                if(_repeatingSoundsDic.TryGetValue(reference, out _EMPTYFLOAT))
                {
                    foundReference = true;
                    break;
                }
            }
            
            if(!foundReference)
            {
                _repeatingSoundsDic.Add(reference, Time.time);
            }

            if(CanPlaySound(reference, timeBetween))
            {
                Sound audioObject = _soundsObjPool.Get();
                audioObject.transform.position = targetPosition;
                audioObject.PlaySound(_gameSounds[gameSound]);
            }
        }
    }
    #endregion

    public void PlayUISound(UISound uISound)
    {
        if(!SettingsData.SoundMuted)
        {
            //AudioSource audioSource;
            // if(oneShotUIObject == null)
            // {
            //     oneShotUIObject = new GameObject("UISound");
            //     audioSource = oneShotUIObject.AddComponent<AudioSource>();
            // }
            
            AudioSource audioSource = oneShotUIObject.GetComponent<AudioSource>();
            //audioSource.clip = _uISounds[uISound];
            audioSource.PlayOneShot(_uISounds[uISound]);

            //Destroy(oneShotUIObject, audioSource.clip.length);
        }
    }

    private bool CanPlaySound(object reference, float durationBetween)
    {
        foreach (var currentReference in _repeatingSoundsDic)
        {
            if(currentReference.Key == reference)
            {
                if(currentReference.Value + durationBetween < Time.time)
                {
                    _repeatingSoundsDic[reference] = Time.time;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    [System.Serializable]
    public class GameSoundAudioClip
    {
        public GameSoundType SoundType;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public class UISoundAudioClip
    {
        public UISound SoundType;
        public AudioClip audioClip;
    }
}
