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



    void OnEnable()
    {
        inventoryButton.onClick.AddListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen += HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose += HandleDialogueBoxClose;
    }

    void OnDisable()
    {
        inventoryButton.onClick.RemoveListener(ToggleInventory);
        DialogueUIBehavior.OnDialogueBoxOpen -= HandleDialogueBoxOpen;
        DialogueUIBehavior.OnDialogueBoxClose -= HandleDialogueBoxClose;
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
}
