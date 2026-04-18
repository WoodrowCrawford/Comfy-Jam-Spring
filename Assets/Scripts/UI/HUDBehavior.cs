using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles all the Hud related behavior, such as opening and closing the inventory, updating the health bar, etc.
/// </summary>
public class HUDBehavior : MonoBehaviour
{

    public delegate void HUDEventHandler();
    public static event HUDEventHandler OnRewardsScreenOpen;

    [Header("HUD References")]
    [SerializeField] private GameObject inventoryUI;
    
    
    [SerializeField] private GameObject infoCard;
    [SerializeField] private GameObject promptMessage;
    [SerializeField] private GameObject rewardsScreen;
    [SerializeField] private GameObject wardrobeUI;

    [Header("Buttons")]
    [SerializeField] private Button toggleQuestButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button introCardExitButton;

    [Header("House Scene References")]
    [SerializeField] private GameObject houseScene;
    [SerializeField] private Sprite houseSceneDay;
    [SerializeField] private Sprite houseSceneNight;

    [Header("Bools")]
    public static bool hasShownIntroCard = false;
    public static bool IsRewardsScreenActive = false;

    void OnEnable()
    {

        //Since we are changing scenes a lot, we need to find the buttons in the scene and add listeners to them every time the scene changes, instead of just doing it once in Awake.
        introCardExitButton = System.Array.Find(
        infoCard.GetComponentsInChildren<Button>(true),
        button => button.name == "CloseButton");

        inventoryButton = GameObject.Find("ToggleInventoryButton")?.GetComponent<Button>();
        toggleQuestButton = GameObject.Find("ToggleQuestButton")?.GetComponent<Button>();
        

        

        //First control the sounds for the buttons
        introCardExitButton.onClick.AddListener(() => SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiClickClip1, introCardExitButton.transform, false, 0f, 0f));


        inventoryButton.onClick.AddListener(ToggleInventory);


        DialogueUIBehavior.OnDialogueBoxOpen += HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose += HandleDialogueBoxClose;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard += ShowInfoCard;

        DayCycleManager.OnDayPhaseWantsInitializePlayerName += ShowPromptMessage;
        DayCycleManager.OnDayPhaseWantsToShowHouseSceneDayTime += ShowHouseSceneDayTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneDayTime += HideHouseSceneDayTime;

        DayCycleManager.OnDayPhaseWantsToShowHouseSceneNightTime += ShowHouseSceneNightTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneNightTime += HideHouseSceneNightTime;

        DayCycleManager.OnDayPhaseWantsToShowRewardsScreen += ShowRewardsScreen;
        DayCycleManager.OnDayPhaseWantsToHideRewardsScreen += HideRewardsScreen;

        DayCycleManager.OnDayPhaseWantsToShowWardrobeUI += ShowWardrobeUI;
        DayCycleManager.OnDaypPhaseWantsToHideWardrobeUI += HideWardrobeUI;

        RewardsBehavior.OnRewardsScreenComplete += HideRewardsScreen;

    }

    void OnDisable()
    {
        introCardExitButton.onClick.RemoveAllListeners();



        inventoryButton.onClick.RemoveListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen -= HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose -= HandleDialogueBoxClose;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard -= ShowInfoCard;
        DayCycleManager.OnDayPhaseWantsToShowHouseSceneDayTime -= ShowHouseSceneDayTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneDayTime -= HideHouseSceneDayTime;

        DayCycleManager.OnDayPhaseWantsToShowHouseSceneNightTime -= ShowHouseSceneNightTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneNightTime -= HideHouseSceneNightTime;
        DayCycleManager.OnDayPhaseWantsInitializePlayerName -= ShowPromptMessage;
        DayCycleManager.OnDayPhaseWantsToShowHouseSceneNightTime -= ShowHouseSceneNightTime;
        DayCycleManager.OnDayPhaseWantsToShowRewardsScreen -= ShowRewardsScreen;
        DayCycleManager.OnDayPhaseWantsToHideRewardsScreen -= HideRewardsScreen;
        DayCycleManager.OnDayPhaseWantsToShowWardrobeUI -= ShowWardrobeUI;
        DayCycleManager.OnDaypPhaseWantsToHideWardrobeUI -= HideWardrobeUI;
        RewardsBehavior.OnRewardsScreenComplete -= HideRewardsScreen;
    }
    

    //A function that toggles the inventory UI on and off when the inventory button is clicked.
    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            return;
        }
        

        inventoryUI.SetActive(!inventoryUI.activeSelf);

        if (inventoryUI.activeSelf)
        {
            //play the inventory open sound effect
            SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiInventoryOpenClip, inventoryUI.transform, false, 0f, 0f);
        }
        else
        {
            //play the inventory close sound effect
            SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.uiInventoryCloseClip, inventoryUI.transform, false, 0f, 0f);
        }

    }

    public void HandleDialogueBoxOpen()
    {
        //disable the inventory button when the dialogue box is open
        inventoryButton.interactable = false;

        
    }

    public void HandleDialogueBoxClose()
    {
        //enable the inventory button when the dialogue box is closed
        inventoryButton.interactable = true;

    }

    public void ShowInfoCard()
    {
        if (infoCard == null)
        {
            return;
        }

        infoCard.SetActive(true);
        hasShownIntroCard = true;

        //play the intro card music
        SoundManager.instance.PlayMusicClip(SoundManager.instance.musicObject, SoundManager.instance.introCardMusicClip, SoundManager.instance.musicObject.transform, true, 0f, 0f);
    }

    public void HideInfoCard()
    {
        if (infoCard == null)
        {
            return;
        }

        infoCard.SetActive(false);

        //stop the intro card music
        SoundManager.instance.StopSoundFXClip(SoundManager.instance.introCardMusicClip);
    }

    public void ShowPromptMessage()
    {
        promptMessage.SetActive(true);
    }

    public void HidePromptMessage()
    {
        promptMessage.SetActive(false);
    }

    public void ShowHouseSceneDayTime()
    {
        houseScene.GetComponent<Image>().sprite = houseSceneDay;
        houseScene.SetActive(true);

       
        //play the morning scene music
        SoundManager.instance.PlayMusicClip(SoundManager.instance.musicObject, SoundManager.instance.morningSceneMusicClip, SoundManager.instance.musicObject.transform, true, 0f, 0f);

        
    }

    public void HideHouseSceneDayTime()
    {
        houseScene.SetActive(false);

        //stop the morning scene music
        SoundManager.instance.StopSoundFXClip(SoundManager.instance.morningSceneMusicClip);
    }

    public void ShowHouseSceneNightTime()
    {
        houseScene.GetComponent<Image>().sprite = houseSceneNight;
        houseScene.SetActive(true);

        //play the evening scene music
        SoundManager.instance.PlayMusicClip(SoundManager.instance.musicObject, SoundManager.instance.eveningSceneMusicClip, SoundManager.instance.musicObject.transform, true, 0f, 0f);

    }

    public void HideHouseSceneNightTime()
    {
        houseScene.SetActive(false);

        //stop the evening scene music
        SoundManager.instance.StopSoundFXClip(SoundManager.instance.eveningSceneMusicClip);
    }

    public void ShowRewardsScreen()
    {
        rewardsScreen.SetActive(true);
        IsRewardsScreenActive = true;

        //tell the rewards screen to start the reward calculation process
        OnRewardsScreenOpen?.Invoke();
    }

    public void HideRewardsScreen()
    {
        rewardsScreen.SetActive(false);
        IsRewardsScreenActive = false;
    }

   

    public void ShowWardrobeUI()
    {
        wardrobeUI.gameObject.SetActive(true);
    }

    public void HideWardrobeUI()
    {
        wardrobeUI.SetActive(false);
    }

}
