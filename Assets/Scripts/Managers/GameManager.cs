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

    private Coroutine playSceneMusicCoroutine;

    private void HandleStartButtonClicked()
    {
        SceneManager.LoadScene("ElleScene");
    }

    private void HandleStartButtonSound()
    {
        if (SoundManager.instance == null || startButton == null)
        {
            return;
        }

        SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiClickClip1, startButton.transform, false, 0f, 0f);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnregisterStartButton();

        if (playSceneMusicCoroutine != null)
        {
            StopCoroutine(playSceneMusicCoroutine);
            playSceneMusicCoroutine = null;
        }
    }

    // A function that can change the scene.
    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterStartButton();

        if (playSceneMusicCoroutine != null)
        {
            StopCoroutine(playSceneMusicCoroutine);
        }

        playSceneMusicCoroutine = StartCoroutine(PlaySceneMusicWhenReady(scene.name));
    }

    private void RegisterStartButton()
    {
        UnregisterStartButton();

        startButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
        startButton?.onClick.AddListener(HandleStartButtonClicked);
        startButton?.onClick.AddListener(HandleStartButtonSound);
    }

    private void UnregisterStartButton()
    {
        startButton?.onClick.RemoveListener(HandleStartButtonClicked);
        startButton?.onClick.RemoveListener(HandleStartButtonSound);
        startButton = null;
    }

    private IEnumerator PlaySceneMusicWhenReady(string sceneName)
    {
        Debug.Log("Current scene is: " + sceneName);

        if (sceneName != "MainMenuScene")
        {
            Debug.Log("Current scene is not: " + sceneName);
            playSceneMusicCoroutine = null;
            yield break;
        }

        yield return null;

        while (SoundManager.instance == null)
        {
            yield return null;
        }

        SoundManager.instance.PlayMusicClip(SoundManager.instance.musicObject, SoundManager.instance.startMenuMusicClip, SoundManager.instance.musicObject.transform, true, 0f, 0f);
        playSceneMusicCoroutine = null;
    }
}
