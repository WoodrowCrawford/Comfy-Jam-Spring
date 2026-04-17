using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager instance;

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
        return Mathf.Pow(10f, masterVolume / 20f);
    }

    public float GetSoundFXVolume()
    {
        audioMixer.GetFloat("SFXVolume", out float soundFXVolume);
        return Mathf.Pow(10f, soundFXVolume / 20f);
    }

    public float GetMusicVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        return Mathf.Pow(10f, musicVolume / 20f);
    }

    public float GetUIVolume()
    {
        audioMixer.GetFloat("UIVolume", out float uiVolume);
        return Mathf.Pow(10f, uiVolume / 20f);
    }


    public void SetMasterVolume(float level)
    {
     
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }


    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
    }


    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetUIVolume(float level)
    {
        audioMixer.SetFloat("UIVolume", Mathf.Log10(level) * 20f);
    }
}
