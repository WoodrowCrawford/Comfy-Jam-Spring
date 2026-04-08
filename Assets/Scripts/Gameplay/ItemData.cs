using UnityEngine;

/// <summary>
/// An item data scriptable object that holds all the data for an item in the game
/// Can be used to define item properties, categories, and effects.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    
    public enum ItemCategory
    {
        Material,
        Consumable,
        Powerup,
        Quest
    }

    public enum PowerupType
    {
        None,
        Heal,
        Speed,
        Stamina
    }

    [Header("Basic Info")]
    [SerializeField] private string itemName;


    [SerializeField, TextArea] private string itemDescription;

    
    [SerializeField] private Sprite itemIcon;

    [Space(10)]
    [Header("Item Type")]
    [SerializeField] private ItemCategory itemCategory;

    [SerializeField] private PowerupType powerupType;

    [SerializeField] private int powerAmount = 1;

    
    //Public getters for the item data variables
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public Sprite ItemIcon => itemIcon;
    public ItemCategory Category => itemCategory;
    public PowerupType Powerup => powerupType;
    public int PowerAmount => powerAmount;
   

    //A function that uses the item based on the item category
    public void UseItem()
    {
        switch (itemCategory)
        {
            case ItemCategory.Material:
                Debug.Log(itemName + " is a crafting material.");
                break;

            case ItemCategory.Consumable:
                Debug.Log("Consumed " + itemName + ".");
                break;

            case ItemCategory.Powerup:
                UsePowerup();
                break;

            case ItemCategory.Quest:
                Debug.Log(itemName + " is a quest item.");
                break;

            default:
                Debug.Log("Used item: " + itemName);
                break;
        }
    }


    //A function that uses the powerup effect based on the powerup type
    private void UsePowerup()
    {
        switch (powerupType)
        {
            case PowerupType.Heal:
                Debug.Log(itemName + " restored " + powerAmount + " health.");
                break;

            case PowerupType.Speed:
                Debug.Log(itemName + " increased speed by " + powerAmount + ".");
                break;

            case PowerupType.Stamina:
                Debug.Log(itemName + " restored " + powerAmount + " stamina.");
                break;

            default:
                Debug.Log(itemName + " used a powerup effect.");
                break;
        }
    }
}
