using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Handles all the Hud related behavior, such as opening and closing the inventory, updating the health bar, etc.
/// </summary>
public class HUDBehavior : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private GameObject infoCard;
    [SerializeField] private GameObject promptMessage;

    [Header("House Scene References")]
    [SerializeField] private GameObject houseScene;
    [SerializeField] private Sprite houseSceneDay;
    [SerializeField] private Sprite houseSceneNight;

    [Header("Bools")]
    public static bool hasShownIntroCard = false;

    void OnEnable()
    {
        inventoryButton.onClick.AddListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen += HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose += HandleDialogueBoxClose;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard += ShowInfoCard;

        DayCycleManager.OnDayPhaseWantsInitializePlayerName += ShowPromptMessage;
        DayCycleManager.OnDayPhaseWantsToShowHouseSceneDayTime += ShowHouseSceneDayTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneDayTime += HideHouseSceneDayTime;

        DayCycleManager.OnDayPhaseWantsToShowHouseSceneNightTime += ShowHouseSceneNightTime;
        DayCycleManager.OnDayPhaseWantsToHideHouseSceneNightTime += HideHouseSceneNightTime;
    }

    void OnDisable()
    {
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
    }
    

    //A function that toggles the inventory UI on and off when the inventory button is clicked.
    public void ToggleInventory()
    {
        if (inventoryUI == null)
        {
            return;
        }

        inventoryUI.SetActive(!inventoryUI.activeSelf);
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
    }

    public void HideInfoCard()
    {
        if (infoCard == null)
        {
            return;
        }

        infoCard.SetActive(false);
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
    }

    public void HideHouseSceneDayTime()
    {
        houseScene.SetActive(false);
    }

    public void ShowHouseSceneNightTime()
    {
        houseScene.GetComponent<Image>().sprite = houseSceneNight;
        houseScene.SetActive(true);
    }

    public void HideHouseSceneNightTime()
    {
        houseScene.SetActive(false);
    }

}
