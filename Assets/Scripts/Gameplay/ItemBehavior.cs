using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


/// <summary>
/// Handles the behavior of individual items in the game, allowing them to be added to the inventory and used by the player.
/// Uses the ItemData scriptable object to store the data for each item.
/// </summary>
public class ItemBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   [Header("Item Data")]
   [SerializeField] private ItemData itemData;
   [SerializeField] private string itemName;
   [SerializeField] private bool canGoInInventory = true;
   public UnityEvent OnItemUsed;


   private Canvas parentCanvas;
   private CanvasGroup canvasGroup;
   private InventorySlot currentSlot;
   private RectTransform inventoryRect;
   private bool isBeingDeleted;

   //public getters for the item data, item name, and current slot
   public ItemData ItemData => itemData;
   public string ItemName => itemName;
   public InventorySlot CurrentSlot => currentSlot;
   public bool CanGoInInventory => canGoInInventory;

   //public UnityEvent OnItemUsed;


    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Start()
    {
        //we need to get the item data from the scriptable object and set it to the item data variable
        SetItemData(itemData);
        currentSlot = GetComponentInParent<InventorySlot>();

        if (currentSlot != null)
        {
            //if we have a current slot, we need to get the inventory rect transform from the parent of the slot
            inventoryRect = currentSlot.transform.parent as RectTransform;
        }
    }


    /// A function that gets the item data from the scriptable object and returns it when called.
    public ItemData GetItemData()
    {
        //we need to return the item data when we call this function
        return itemData;
    }

    //A function used to set the item data
    public void SetItemData(ItemData newItemData)
    {
        if (newItemData == null)
        {
            return;
        }

        //we need to set the item data when we call this function
        itemData = newItemData;

        itemName = itemData.ItemName;


        //get the image component of the item and set the sprite to the item icon
        Image itemImage = GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = itemData.ItemIcon;
        }
    }

    //A function that sets the current slot of the item to the given slot.
    public void SetCurrentSlot(InventorySlot newSlot)
    {
        currentSlot = newSlot;
    }


    //A function that gets the item data from the scriptable object and uses it
    public void UseItem()
    {
        if (itemData == null)
        {
            return;
        }

        //we need to use the item when we call this function
        Debug.Log("Used item: " + itemData.ItemName);
        itemData.UseItem();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isBeingDeleted = false;
        currentSlot = GetComponentInParent<InventorySlot>();

        //if we have a current slot, we need to get the inventory rect transform from the parent of the slot
        if (currentSlot != null)
        {
            inventoryRect = currentSlot.transform.parent as RectTransform;
        }

        canvasGroup.blocksRaycasts = false;

        if (parentCanvas != null)
        {
            transform.SetParent(parentCanvas.transform, true);
        }
    }


    //A function that moves the item with the mouse when it is being dragged.
    public void OnDrag(PointerEventData eventData)
    {
        if (isBeingDeleted)
        {
            return;
        }

        transform.position = eventData.position;
    }


    //A function that checks if the item is dropped outside of the inventory area, and deletes it if it is.
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isBeingDeleted)
        {
            return;
        }

        canvasGroup.blocksRaycasts = true;

        bool droppedOutsideInventory = inventoryRect != null &&
            !RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, eventData.position, eventData.pressEventCamera);

        if (droppedOutsideInventory)
        {
            DeleteDraggedItem();
            return;
        }
    }


    //A function to delete the item if it is dragged outside of the inventory area.
    private void DeleteDraggedItem()
    {
        isBeingDeleted = true;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        if (currentSlot != null)
        {
            //we need to subtract the points from the player when they delete an item by dragging it outside of the inventory
            PlayerBehavior player = FindObjectOfType<PlayerBehavior>();
            player?.SubtractPoints(currentSlot.CurrentItem.GetComponent<ItemBehavior>().ItemData.ItemPoints);


            currentSlot.ClearCurrentItem();
        }

        Debug.Log("Item dragged outside inventory and deleted: " + gameObject.name);
        Destroy(gameObject);
    }
}
