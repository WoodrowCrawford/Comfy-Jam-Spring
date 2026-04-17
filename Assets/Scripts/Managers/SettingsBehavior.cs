using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;


    [Header("Sound Settings")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _uiVolumeSlider;


    //settings getters

   




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



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }





    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        //if the scene is the main menu
        if (scene == SceneManager.GetSceneByBuildIndex(0))
        {
            //get a list of all the buttons in the scene 
            Button[] allGameObjects = Resources.FindObjectsOfTypeAll<Button>();

            //get a list of all the sliders in the scene 
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();

            foreach (Slider slider in allSliders)
            {
                string sliderName = slider.name.Trim();
               
                //if the slider is the master volume slider
                if (sliderName == "MasterVolume")
                {
                    //set the master volume slider to be the slider
                    _masterVolumeSlider = slider;

                    //set the slider value to the current master volume
                    _masterVolumeSlider.value = SoundMixerManager.instance.GetMasterVolume();

                    //add a listener to the slider
                    _masterVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMasterVolume);
                }

                //if the slider is the music volume slider
                if (sliderName == "MusicVolume")
                {
                    //set the music volume slider to be the slider
                    _musicVolumeSlider = slider;

                    //get the current music volume
                    _musicVolumeSlider.value = SoundMixerManager.instance.GetMusicVolume();

                    //add a listener to the slider
                    _musicVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMusicVolume);
                }

                //if the slider is the sound fx volume slider
                if (sliderName == "SoundFXVolume")
                {
                    //set the sound fx volume slider to be the slider
                    _sfxVolumeSlider = slider;

                    //get the current sound fx volume
                    _sfxVolumeSlider.value = SoundMixerManager.instance.GetSoundFXVolume();

                    //add a listener to the slider
                    _sfxVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetSoundFXVolume);
                }

                //if the slider is the ui volume slider
                if (sliderName == "UiSoundFXVolume")
                {
                    //set the ui volume slider to be the slider
                    _uiVolumeSlider = slider;

                    //get the current ui volume
                    _uiVolumeSlider.value = SoundMixerManager.instance.GetUIVolume();

                    //add a listener to the slider
                    _uiVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetUIVolume);
                }
            }
        }

        //else if the scene is the bedroom scene
        else if (scene == SceneManager.GetSceneByBuildIndex(1))
        {
            
            //get a list of all the sliders in the scene
            Slider[] allSliders = Resources.FindObjectsOfTypeAll<Slider>();


            foreach (Slider slider in allSliders)
            {
                string sliderName = slider.name.Trim();
                
                //if the slider is the master volume slider
                if (sliderName == "MasterVolume")
                {
                    //set the master volume slider to be the slider
                    _masterVolumeSlider = slider;

                    //set the slider value to the current master volume
                    _masterVolumeSlider.value = SoundMixerManager.instance.GetMasterVolume();

                    //add a listener to the slider
                    _masterVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMasterVolume);
                }

                //if the slider is the music volume slider
                if (sliderName == "MusicVolume")
                {
                    //set the music volume slider to be the slider
                    _musicVolumeSlider = slider;

                    //get the current music volume
                    _musicVolumeSlider.value = SoundMixerManager.instance.GetMusicVolume();

                    //add a listener to the slider
                    _musicVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetMusicVolume);
                }

                //if the slider is the sound fx volume slider
                if (sliderName == "SFXVolume")
                {
                    //set the sound fx volume slider to be the slider
                    _sfxVolumeSlider = slider;

                    //get the current sound fx volume
                    _sfxVolumeSlider.value = SoundMixerManager.instance.GetSoundFXVolume();

                    //add a listener to the slider
                    _sfxVolumeSlider.onValueChanged.AddListener(SoundMixerManager.instance.SetSoundFXVolume);
                }
            }


        }
    }
}