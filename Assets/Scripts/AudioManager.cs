using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
    

//Using enums to make it faster to select the correct audio while calling PlayAudio from scripts
public enum Audio
{
    IncreaseBet,
    DecreaseBet,
    Click,
    SpinStart,
    SpinStop,
    WinResult,
    Error,
    Chips
}

public class AudioManager : Singleton<AudioManager>
{
    List<AudioSource> _audioSources = new List<AudioSource>();

    [SerializeField]
    private AudioClip _increaseBetAudio;
    [SerializeField]
    private AudioClip _decreaseBetAudio; 
    [SerializeField]
    private AudioClip _ClickedAudio;  
    [SerializeField]
    private AudioClip _SpinStartAudio;
    [SerializeField]
    private AudioClip _SpinStopAudio; 
    [SerializeField]
    private AudioClip _WinResultAudio; 
    [SerializeField]
    private AudioClip _ErrorAudio; 
    [SerializeField]
    private AudioClip _ChipsAudio; 

    void Start()
    {
      _audioSources.Add(gameObject.AddComponent<AudioSource>());   
    }

    /// <summary>
    /// Plays the selected audio
    /// </summary>
    /// <param name="audioclip"> Enum of audioclip</param>
    public void PlayAudio(Audio audioclip)
    {
        AudioSource selectedAudioSource = GetAudioSource();
        AudioClip selectedAudioClip = GetAudioClip(audioclip);
        selectedAudioSource.clip = selectedAudioClip;
        selectedAudioSource.Play();
    }

    /// <summary>
    /// Select and return and idle AudioSource, if non, creates one.
    /// </summary>
    /// <returns>AudioSource</returns>
    private AudioSource GetAudioSource()
    {
        for (int i = 0; i < _audioSources.Count; i++)
        {
            if (!_audioSources[i].isPlaying)
            {
                return _audioSources[i];
            }
        }
        
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();

        _audioSources.Add(newAudioSource);
        return newAudioSource;
    }

    /// <summary>
    /// Selects the audioclip based on the Audio enum
    /// </summary>
    /// <param name="audioclip"></param>
    /// <returns>AudioClip</returns>
    private AudioClip GetAudioClip(Audio audioclip)
    {
        switch (audioclip)
        {
            case Audio.IncreaseBet:
                return _increaseBetAudio;
            
            case Audio.DecreaseBet:
                return _decreaseBetAudio;
            
            case Audio.Click:
                return _ClickedAudio;
            
            case Audio.SpinStart:
                return _SpinStartAudio;
            
            case Audio.SpinStop:
                return _SpinStopAudio;
            
            case Audio.WinResult:
                return _WinResultAudio;

            case Audio.Error:
                return _ErrorAudio;
            
            case Audio.Chips:
                return _ChipsAudio;

            default:
                Debug.LogWarning("Audio not found");
                return null;
        }
    }

}
