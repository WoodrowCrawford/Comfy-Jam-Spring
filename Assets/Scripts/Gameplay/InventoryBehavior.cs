using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


/// <summary>
/// Handles the inventory system for the player, allowing them to collect and manage items throughout the game.
/// </summary>
public class InventoryBehavior : MonoBehaviour, IPointerClickHandler
{
    public delegate void InventoryEvent(GameObject item);
    public static event InventoryEvent OnItemSuccessfullyAddedToInventory;

    [Header("Inventory References")]
    [SerializeField] private GameObject inventoryUI;

    [Space(10)]
    [Header("Slot Data")]
    [SerializeField] private List<GameObject> inventorySlots = new List<GameObject>();


    void OnEnable()
    {
        ItemPickup.OnItemPickup += HandleItemPickup;
        DayCycleManager.OnNewDay += ClearInventory;
    }

    void OnDisable()
    {
        ItemPickup.OnItemPickup -= HandleItemPickup;
        DayCycleManager.OnNewDay -= ClearInventory;
    }

    private void HandleItemPickup(GameObject itemToPickUp)
    {
        if (itemToPickUp == null)
        {
            return;
        }

        // Keep inventory items independent from scene items.
        GameObject inventoryItem = Instantiate(itemToPickUp);
        PrepareInventoryItem(inventoryItem);
        bool wasAdded = AddItemToInventory(inventoryItem);

        if (wasAdded)
        {

           //if we picked up a bug, we want to play the bug catch sound effect
            if(itemToPickUp.GetComponent<ItemBehavior>().ItemData.Category == ItemData.ItemCategory.Bug)
            {
                SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.bugCatchClip1, itemToPickUp.transform, false, 0f, 0f);
            }

            else if(itemToPickUp.GetComponent<ItemBehavior>().ItemData.Category == ItemData.ItemCategory.Plant)
            {
                SoundManager.instance.PlaySoundFXClip(SoundManager.instance.soundFXObject, SoundManager.instance.pickHerbClip1, itemToPickUp.transform, false, 0f, 0f);
            }
            
            Destroy(itemToPickUp);

            //fire an event to update the player weekly points when they pick up an item
            OnItemSuccessfullyAddedToInventory?.Invoke(inventoryItem);
        }
        else
        {
            Destroy(inventoryItem);
            Debug.Log("Inventory full, could not pick up: " + itemToPickUp.name);
        }
    }



    private void PrepareInventoryItem(GameObject inventoryItem)
    {
        if (inventoryItem == null)
        {
            return;
        }

        // Remove world-only behavior so inventory copies are not affected by scene logic.
        ItemPickup pickup = inventoryItem.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            Destroy(pickup);
        }

        BugAI bugAI = inventoryItem.GetComponent<BugAI>();
        if (bugAI != null)
        {
            Destroy(bugAI);
        }

        Rigidbody2D rb = inventoryItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }

        Collider2D collider = inventoryItem.GetComponent<Collider2D>();
        if (collider != null)
        {
            Destroy(collider);
        }

        inventoryItem.tag = "Untagged";
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObject == null)
        {
            return;
        }

        ItemBehavior itemBehavior = clickedObject.GetComponentInParent<ItemBehavior>();
        if (itemBehavior != null)
        {
            itemBehavior.UseItem();
        }
    }

    public void Start()
    {
        //we need to get the inventory slots when the game starts
        GetInventorySlots();

        
    }

    public void GetInventorySlots()
    {
        if (inventoryUI == null)
        {
            return;
        }

        //we need to get all the inventory slots in the game and add them to the list
        inventorySlots.Clear();
        foreach (Transform child in inventoryUI.transform)
        {
            inventorySlots.Add(child.gameObject);

            InventorySlot slotBehavior = child.GetComponent<InventorySlot>();
            if (slotBehavior != null)
            {
                slotBehavior.RefreshCurrentItem();
            }
        }
    }
    

    //A function that clears the inventory, by clearing all the inventory slots of their current items. 
    public void ClearInventory()
    {
        Debug.Log("Clearing inventory for new day.");

        if (inventorySlots.Count == 0)
        {
            GetInventorySlots();
        }

        //we need to clear all the inventory slots
        foreach (GameObject inventorySlot in inventorySlots)
        {
            InventorySlot slotBehavior = inventorySlot.GetComponent<InventorySlot>();
            if (slotBehavior != null)
            {
                slotBehavior.ClearCurrentItem();
            }
        }
    }


    //A function that adds an item to the inventory, by finding the first empty slot and placing the item there.

    public bool AddItemToInventory(GameObject itemToAdd)
    {
        if (itemToAdd == null)
        {
            return false;
        }

        if (inventorySlots.Count == 0)
        {
            GetInventorySlots();
        }

        //we need to add the item to the first empty inventory slot
        foreach (GameObject inventorySlot in inventorySlots)
        {
            InventorySlot slotBehavior = inventorySlot.GetComponent<InventorySlot>();
            if (slotBehavior == null)
            {
                continue;
            }

            slotBehavior.RefreshCurrentItem();
            if (slotBehavior.IsEmpty)
            {
                slotBehavior.SetCurrentItem(itemToAdd);
                
                //set the size pf thje item to fit the slot
                RectTransform itemRect = itemToAdd.GetComponent<RectTransform>();
                RectTransform slotRect = inventorySlot.GetComponent<RectTransform>();
                if (itemRect != null && slotRect != null)  
                {             
                    itemRect.sizeDelta = slotRect.sizeDelta;
                }

                Debug.Log("Added item to inventory slot: " + inventorySlot.name);
                return true;
            }
        }

        return false;
    }
}