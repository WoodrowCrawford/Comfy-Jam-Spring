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

    [Header("Bools")]
    [SerializeField] private bool hasShownIntroCard = false;


    void OnEnable()
    {
        inventoryButton.onClick.AddListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen += HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose += HandleDialogueBoxClose;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard += ShowInfoCard;
    }

    void OnDisable()
    {
        inventoryButton.onClick.RemoveListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen -= HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose -= HandleDialogueBoxClose;

        DayCycleManager.OnDayPhaseWantsToShowInfoCard -= ShowInfoCard;
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
}
