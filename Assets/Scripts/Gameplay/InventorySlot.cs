using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// A script that represents a single slot in the players inventory. Can hold one item at a time.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    //The current held item in this slot.
    [SerializeField] private GameObject currentItem;

    //Public getter for the current item in this slot, and a boolean to check if the slot is empty.
    public GameObject CurrentItem => currentItem;
    public bool IsEmpty => currentItem == null;


    
    private void Awake()
    {
        //On awake, refresh the current item in the slot.
        RefreshCurrentItem();
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

        //in each item slot in the inventory, check if there is an item in the slot, and if there is, set the current item to that item
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ItemBehavior>() != null)
            {
                currentItem = child.gameObject;
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
            return;
        }

        currentItem.transform.SetParent(transform, false);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localScale = new Vector3(1f, 1f, 1f);

        ItemBehavior itemBehavior = currentItem.GetComponent<ItemBehavior>();
        if (itemBehavior != null)
        {
            itemBehavior.SetCurrentSlot(this);
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
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null)
        {
            return;
        }

        ItemBehavior droppedItemBehavior = droppedItem.GetComponent<ItemBehavior>();
        if (droppedItemBehavior == null)
        {
            return;
        }

        InventorySlot previousSlot = droppedItemBehavior.CurrentSlot;
        GameObject previousItem = currentItem;

        if (previousSlot == this)
        {
            SetCurrentItem(droppedItem);
            return;
        }

        SetCurrentItem(droppedItem);

        if (previousSlot != null)
        {
            if (previousItem != null && previousItem != droppedItem)
            {
                previousSlot.SetCurrentItem(previousItem);
            }
            else
            {
                previousSlot.ClearCurrentItem();
            }
        }
    }
}
