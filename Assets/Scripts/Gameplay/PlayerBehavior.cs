using UnityEngine;

/// <summary>
/// Will contain all the behaviors for the player such as name, health and other stats.
/// </summary>
public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private int weeklyPoints;

    //public getter for the player name
    public string PlayerName => playerName;
    public int WeeklyPoints => weeklyPoints;

    void OnEnable()
    {
        PromptMessageBehavior.OnNameCreated += SetPlayerName;
        ItemPickup.OnItemPickup += UpdateWeeklyPoints;
        DayCycleManager.OnWeekReset += ResetWeeklyPoints;
    }

    void OnDisable()
    {
        PromptMessageBehavior.OnNameCreated -= SetPlayerName;
        ItemPickup.OnItemPickup -= UpdateWeeklyPoints;
        DayCycleManager.OnWeekReset -= ResetWeeklyPoints;
    }


    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    //this function will update the player's weekly points when they pick up an item
    private void UpdateWeeklyPoints(GameObject itemToPickup)
    {
        if (itemToPickup == null)
        {
            return;
        }

        ItemBehavior itemBehavior = itemToPickup.GetComponent<ItemBehavior>();
        if (itemBehavior != null && itemBehavior.ItemData != null)
        {
            weeklyPoints += itemBehavior.ItemData.ItemPoints;
        }
    }

    public void ResetWeeklyPoints()
    {
        weeklyPoints = 0;
    }

  
}
