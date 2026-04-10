using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// A script that represents a single slot in the players inventory. Can hold one item at a time.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    //The current held item in this slot.
    [SerializeField] private GameObject currentItem;

    [SerializeField] private TMP_Text itemNameText;

    //Public getter for the current item in this slot, and a boolean to check if the slot is empty.
    public GameObject CurrentItem => currentItem;
    public bool IsEmpty => currentItem == null;


    
    private void Awake()
    {
        //On awake, refresh the current item in the slot.
        RefreshCurrentItem();
        itemNameText = GetComponentInChildren<TMP_Text>();
        
    }
    

    //This function is called whenever the children of this slot change, which means we need to refresh the current item in the slot.
    private void OnTransformChildrenChanged()
    {
        RefreshCurrentItem();
    }


    //A function that refreshes the current item in the slot.
    public void RefreshCurrentItem()
    {
        //set the current item to null
        currentItem = null;

        //set the item name text to be empty
        itemNameText.text = string.Empty;

        //in each item slot in the inventory, check if there is an item in the slot, and if there is, set the current item to that item
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ItemBehavior>() != null)
            {
                //set the current item and name to the slot
                currentItem = child.gameObject;
                itemNameText.text = currentItem.GetComponent<ItemBehavior>().ItemName;
                break;
            }
        }
    }


    //A function that sets the current item in the slot to the given item, and makes the item a child of the slot.
    public void SetCurrentItem(GameObject item)
    {
        currentItem = item;

        if (currentItem == null)
        {
            itemNameText.text = string.Empty;
            return;
        }

        currentItem.transform.SetParent(transform, false);
        SnapItemToSlot(currentItem);

        ItemBehavior itemBehavior = currentItem.GetComponent<ItemBehavior>();
        if (itemBehavior != null)
        {
            itemBehavior.SetCurrentSlot(this);
            itemNameText.text = itemBehavior.ItemName;
        }
    }

    //A function that snaps the given item to the center of the slot, and resets its rotation and scale.
    private void SnapItemToSlot(GameObject item)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();

        if (itemRect != null)
        {
            // Snap the item to the center of the slot
            itemRect.anchoredPosition = Vector2.zero;
            itemRect.localRotation = Quaternion.identity;
            itemRect.localScale = Vector3.one;
        }
        else
        {
            // If the item doesn't have a RectTransform, just reset its local position, rotation, and scale
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
        }
    }

    //A function that clears the current item in the slot.
    public void ClearCurrentItem()
    {
        currentItem = null;
    }

    //This function is called when an item is dropped after being dragged over a slot.
    public void OnDrop(PointerEventData eventData)
    {
        //Get the dropped item from the event data, and check if it has an ItemBehavior component.
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null)
        {
            return;
        }

        //get the item behavior from the dropped item, and if it doesnt have one then return
        ItemBehavior droppedItemBehavior = droppedItem.GetComponent<ItemBehavior>();
        if (droppedItemBehavior == null)
        {
            return;
        }


        //set the previous slot of the dropped item to this slot, and get the previous item in this slot
        InventorySlot previousSlot = droppedItemBehavior.CurrentSlot;
        GameObject previousItem = currentItem;

        //if this is the same slot that the item was dragged from, then we just need to refresh the current item in this slot and return
        if (previousSlot == this)
        {
            SetCurrentItem(droppedItem);
            return;
        }

        //if the previous slot is not null, then we need to clear the current item in the previous slot to prepare for the item being moved to this slot
        if (previousSlot != null)
        {
            previousSlot.ClearCurrentItem();
        }

        //clear the current item in this slot to prepare for the new item being dropped here
        ClearCurrentItem();

        //if the previous slot was not null and the previous item is not null and the previous item is not the same as the dropped item...
        if (previousSlot != null && previousItem != null && previousItem != droppedItem)
        {
            //set the current item in the previous slot to the previous item
            previousSlot.SetCurrentItem(previousItem);
        }

        //set the current item in this slot to the dropped item
        SetCurrentItem(droppedItem);

        //if the previous slot was not null...
        if (previousSlot != null)
        {
            //refresh the previous slot to update its current item and name text
            previousSlot.RefreshCurrentItem();
        }

        //refresh this slot to update its current item and name text
        RefreshCurrentItem();
    }
}
