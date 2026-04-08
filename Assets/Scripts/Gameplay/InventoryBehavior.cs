using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


/// <summary>
/// Handles the inventory system for the player, allowing them to collect and manage items throughout the game.
/// </summary>
public class InventoryBehavior : MonoBehaviour, IPointerClickHandler
{
    [Header("Inventory References")]
    [SerializeField] private GameObject inventoryUI;

    [Space(10)]
    [Header("Slot Data")]
    [SerializeField] private List<GameObject> inventorySlots = new List<GameObject>();


    void OnEnable()
    {
        ItemPickup.OnItemPickup += (itemData) => AddItemToInventory(itemData);
    }

    void OnDisable()
    {
        ItemPickup.OnItemPickup -= (itemData) => AddItemToInventory(itemData);
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


    //A function that adds an item to the inventory, by finding the first empty slot and placing the item there.

    public void AddItemToInventory(GameObject itemToAdd)
    {
        if (itemToAdd == null)
        {
            return;
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
                break;
            }
        }
    }
}