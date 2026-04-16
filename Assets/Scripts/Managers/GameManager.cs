using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private Button startButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        
    }


    
    private void OnEnable()
    {
        //find the start button in the scene and add a listener to it to start the game when clicked
        startButton = GameObject.Find("PlayButton")?.GetComponent<Button>();

        startButton?.onClick.AddListener(() => SceneManager.LoadScene("ElleScene"));
        startButton?.onClick.AddListener(() => SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiClickClip1, startButton.transform, false, 0f, 0f));
    }

    
    private void OnDisable()
    {
        startButton?.onClick.RemoveListener(() => SceneManager.LoadScene("ElleScene"));
        startButton?.onClick.RemoveListener(() => SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiClickClip1, startButton.transform, false, 0f, 0f));
    }


    //A function that can change the scene
    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);  
    }


    
}
