using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;



//A script that controls the creation on sounds in the game with different sound types

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Sound types")]
    public AudioSource musicObject;
    public AudioSource soundFXObject;
    public AudioSource uiSoundObject;


    [Header("Music Clips")]
    public AudioClip startMenuMusicClip;
    public AudioClip morningSceneMusicClip;
    public AudioClip introCardMusicClip;
    public AudioClip explorationMusicClip;
    public AudioClip eveningSceneMusicClip;



    [Header("UI Clips")]
    public AudioClip uiBoopClip1;
    public AudioClip uiBoopClip2;
    public AudioClip uiBoopClip3;
    public AudioClip uiBoopClip4;
    public AudioClip uiClickClip1;
    public AudioClip uiClickClip2;
    public AudioClip uiClickClip3;
    public AudioClip uiClickClip4;
    public AudioClip uiClickClip5;
    public AudioClip uiInventoryOpenClip;
    public AudioClip uiInventoryCloseClip;
    

    [Header("List clips")]
    public AudioClip listOpenClip1;
    public AudioClip listOpenClip2;
    public AudioClip listCloseClip;

    [Header("Bug catch clips")]
    public AudioClip bugCatchClip1;
    public AudioClip bugCatchClip2;
    public AudioClip bugCatchClip3;


    [Header("Pick herb clips")]
    public  AudioClip pickHerbClip1;
    public  AudioClip pickHerbClip2;
    public  AudioClip pickHerbClip3;

    [Header("Walk clips")]
    public AudioClip walkGrassClip;
    public AudioClip walkMudClip;
    public AudioClip walkTilesClip;



    


   

    
    public void PauseAudioListener() => AudioListener.pause = true;
    public void UnpauseAudioListener() => AudioListener.pause = false;


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

    void OnEnable()
    {
        //subscribes to the events
        //PauseSystem.onGamePaused += PauseAudioListener;
        //PauseSystem.onGameUnpaused += UnpauseAudioListener;

    }

    void OnDisable()
    {
        //unsubscribes to the events
        //PauseSystem.onGamePaused -= PauseAudioListener;
        //PauseSystem.onGameUnpaused -= UnpauseAudioListener;
    }


   

    public void PlayMusicClip(AudioSource audioSourceType, AudioClip audioClip, Transform spawnTransform, bool loop, float spatialBlend, float spread)
    {

        //check if there is already a music object in the scene and if there is, stop it before playing the new music to avoid multiple music objects playing at once
        GameObject existingMusicObject = GameObject.Find("MusicObject(Clone)");
        if(existingMusicObject != null)
        {
            //destroy the existing music object before we play the new music to avoid multiple music objects playing at once
            Destroy(existingMusicObject);
        }
        
        //destroy the existing music object before we play the new music to avoid multiple music objects playing
        Debug.Log("A music object is already playing, stopping it before playing the new music.");
        

        Debug.Log("Playing music clip: " + audioClip.name);


        //spawn in game object
        AudioSource audioSource = Instantiate(audioSourceType, spawnTransform.position, Quaternion.identity);

      

        //if the sound type is ui sound
        if (audioSourceType == uiSoundObject)
        {
            //make it so that the audio source doesnt stop when the game pauses
            audioSource.ignoreListenerPause = true;
        }


       
        //assign audio clip
        audioSource.clip = audioClip;

        //assign the audio loop
        audioSource.loop = loop;

        //assign the audio spatial blend
        audioSource.spatialBlend = spatialBlend;
        audioSource.spread = spread;

        //play sound
        audioSource.Play();


        //if loop is not enabled
        if (!loop)
        {
            //get length of sound FX clip
            float clipLength = audioSource.clip.length;

            //destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            return;
        }
    }

    



    public void PlaySoundFXClip(AudioSource audioSourceType, AudioClip audioClip, Transform spawnTransform, bool loop, float spatialBlend, float spread)
    {
        //spawn in game object
        AudioSource audioSource = Instantiate(audioSourceType, spawnTransform.position, Quaternion.identity);

      

        //if the sound type is ui sound
        if (audioSourceType == uiSoundObject)
        {
            //make it so that the audio source doesnt stop when the game pauses
            audioSource.ignoreListenerPause = true;
        }


       
        //assign audio clip
        audioSource.clip = audioClip;

        //assign the audio loop
        audioSource.loop = loop;

        //assign the audio spatial blend
        audioSource.spatialBlend = spatialBlend;
        audioSource.spread = spread;

        //play sound
        audioSource.Play();


        //if loop is not enabled
        if (!loop)
        {
            //get length of sound FX clip
            float clipLength = audioSource.clip.length;

            //destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            return;
        }
    }


   

    public void PlaySoundFXClipAtSetVolume(AudioSource audioSourceType, AudioClip audioClip, Transform spawnTransform, bool loop, float spatialBlend, float spread, float volume)
    {
        //spawn in game object
        AudioSource audioSource = Instantiate(audioSourceType, spawnTransform.position, Quaternion.identity);

      

        //if the sound type is ui sound
        if (audioSourceType == uiSoundObject)
        {
            //make it so that the audio source doesnt stop when the game pauses
            audioSource.ignoreListenerPause = true;
        }

        //assign audio clip
        audioSource.clip = audioClip;

        //assign the audio loop
        audioSource.loop = loop;


        //assign the audio spatial blend
        audioSource.spatialBlend = spatialBlend;

        audioSource.spread = spread;


        //assign the volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //if loop is not enabled
        if (!loop)
        {
            //get length of sound FX clip
            float clipLength = audioSource.clip.length;

            //destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            return;
        }
    }


   

    public void PlaySoundFXClipAtSetVolumeAndRange(AudioSource audioSourceType, AudioClip audioClip, Transform spawnTransform, bool loop, float spatialBlend, float spread, float minRange, float maxRange, float volume)
    {
        //spawn in game object
        AudioSource audioSource = Instantiate(audioSourceType, spawnTransform.position, Quaternion.identity);

        

        //if the sound type is ui sound
        if (audioSourceType == uiSoundObject)
        {
            //make it so that the audio source doesnt stop when the game pauses
            audioSource.ignoreListenerPause = true;
        }

        //assign audio clip
        audioSource.clip = audioClip;

        //assign the audio loop
        audioSource.loop = loop;


        //assign the audio spatial blend
        audioSource.spatialBlend = spatialBlend;

        audioSource.spread = spread;

        audioSource.minDistance = minRange;

        audioSource.maxDistance = maxRange;


        //assign the volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //if loop is not enabled
        if (!loop)
        {
            //get length of sound FX clip
            float clipLength = audioSource.clip.length;

            //destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);
        }
        else
        {
            return;
        }
    }



    public void PlaySoundFXClipAndAttachToGameObject(AudioSource audioSourceType, AudioClip audioClip, Transform spawnTransform, bool loop, float spatialBlend, float spread, float volume, GameObject gameObjectToAttachTo)
    {
        //spawn in game object
        AudioSource audioSource = Instantiate(audioSourceType, spawnTransform.position, Quaternion.identity);

        //Attach the audio source to the game object
        audioSource.transform.SetParent(gameObjectToAttachTo.transform);


    

        //if the sound type is ui sound
        if (audioSourceType == uiSoundObject)
        {
            //make it so that the audio source doesnt stop when the game pauses
            audioSource.ignoreListenerPause = true;
        }


        //assign audio clip
        audioSource.clip = audioClip;

        //assign the audio loop
        audioSource.loop = loop;

        //assign the audio spatial blend
        audioSource.spatialBlend = spatialBlend;

        audioSource.spread = spread;

        //assign the volume
        audioSource.volume = volume;


        //play sound
        audioSource.Play();
        

        //if loop is not enabled
        if (!loop)
        {
            //get length of sound FX clip
            float clipLength = audioSource.clip.length;

            //destroy the clip after it is done playing
            Destroy(audioSource.gameObject, clipLength);

           
        }
        else
        {
            return;
        }
    }





    public void StopSoundFXClip(AudioClip audioClip)
    {
        //find the audio source with the given audio clip
        AudioSource[] audioSourceToFindWithClip = FindObjectsByType<AudioSource>();

        //loop through all the audio sources
        foreach (AudioSource audioSource in audioSourceToFindWithClip)
        {
            if (audioSource.clip == audioClip)
            {
                //stop the audio source
                audioSource.Stop();

                //destroy the audio source
                Destroy(audioSource.gameObject);
            }
        }
    }

    public void StopSoundsInArray(AudioClip[] audioClips)
    {
        //find the audio source with the given audio clip
        AudioSource[] audioSourceToFindWithClip = FindObjectsByType<AudioSource>();

        foreach (AudioSource audioSource in audioSourceToFindWithClip) 
        {
            audioSource.Stop();

            Destroy(audioSource.gameObject);

        }
    }


    public bool IsSoundFXClipPlaying(AudioClip audioClip)
    {
        //get a list of all the audio sources
        AudioSource[] audioSources = FindObjectsByType<AudioSource>();

        //loop through all the audio sources
        foreach (AudioSource audioSource in audioSources)
        {
            //check if the audio source clip is the same as the selected audio source clip and if it is playing
            if (audioSource.clip == audioClip && audioSource.isPlaying)
            {
                //the sound is playing
                return true;
            } 
        }

        //the sound is not playing
        return false;
    }


    public bool IsSoundObjectPlaying(AudioSource audioSourceType)
    {
        //get a list of all the audio sources
        AudioSource[] audioSources = FindObjectsByType<AudioSource>();

        //loop through all the audio sources
        foreach (AudioSource audioSource in audioSources)
        {
            //check if the audio source is the same as the selected audio source and if it is playing
            if (audioSource == audioSourceType && audioSource.isPlaying)
            {
                //the sound is playing
                return true;
            }
        }

        //the sound is not playing
        return false;
    }
}