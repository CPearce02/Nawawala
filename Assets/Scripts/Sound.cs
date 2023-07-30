using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioMixer _audioMixer;

    public void PlaySound(AudioClip targetAudioClip)
    {
        _audioSource.clip = targetAudioClip;

        PlayAudio();
        Invoke("ReleaseMe", targetAudioClip.length);
    }

    public void PlaySound(AudioClip targetAudioClip, bool repeating)
    {
        _audioSource.clip = targetAudioClip;
        _audioSource.loop = repeating;

        PlayAudio();
        Invoke("ReleaseMe", targetAudioClip.length);
    }

    public void PlaySound(AudioClip targetAudioClip, bool repeating, AudioMixer _targetrAudioMixer)
    {
        _audioSource.clip = targetAudioClip;
        _audioMixer = _targetrAudioMixer;
        _audioSource.loop = repeating;

        PlayAudio();
        Invoke("ReleaseMe", targetAudioClip.length);
    }

    public void PlayRepeatingSound()
    {
        
    }

    private void PlayAudio()
    {
        _audioSource.Play();
    }

    private void ReleaseMe()
    {
        SoundManager.Instance.Release3DSound(this);
    }
}
