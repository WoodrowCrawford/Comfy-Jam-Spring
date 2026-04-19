using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager instance;

    private const float MinLinearVolume = 0.0001f;
    private const float MinDecibelVolume = -80f;

    [SerializeField] private AudioMixer audioMixer;
    


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public float GetMasterVolume()
    {
        audioMixer.GetFloat("MasterVolume", out float masterVolume);
        if (masterVolume <= MinDecibelVolume)
        {
            return 0f;
        }
        return Mathf.Pow(10f, masterVolume / 20f);
    }

    public float GetSoundFXVolume()
    {
        audioMixer.GetFloat("SFXVolume", out float soundFXVolume);
        if (soundFXVolume <= MinDecibelVolume)
        {
            return 0f;
        }
        return Mathf.Pow(10f, soundFXVolume / 20f);
    }

    public float GetMusicVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        if (musicVolume <= MinDecibelVolume)
        {
            return 0f;
        }
        return Mathf.Pow(10f, musicVolume / 20f);
    }

    public float GetUIVolume()
    {
        audioMixer.GetFloat("UIVolume", out float uiVolume);
        if (uiVolume <= MinDecibelVolume)
        {
            return 0f;
        }
        return Mathf.Pow(10f, uiVolume / 20f);
    }


    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", ToDecibels(level));
    }


    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", ToDecibels(level));
    }


    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", ToDecibels(level));
    }

    public void SetUIVolume(float level)
    {
        audioMixer.SetFloat("UIVolume", ToDecibels(level));
    }

    private float ToDecibels(float level)
    {
        if (level <= MinLinearVolume)
        {
            return MinDecibelVolume;
        }

        return Mathf.Log10(level) * 20f;
    }
}
