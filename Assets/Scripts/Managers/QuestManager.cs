using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] QuestData questData;

    public List<ItemData> currentRequiredItems = new List<ItemData>();
    public Dictionary<ItemData, int> collectionStatus = new Dictionary<ItemData, int>();

    void Awake() => Instance = this;

    void OnEnable()
    {
        DayCycleManager.OnNewDay += GenerateDailyQuest;
        ItemPickup.OnItemPickup += CheckItemAgainstQuest;
    }

    public delegate void QuestEvent();
    public static event QuestEvent OnQuestUpdated;

    public struct QuestItemStatus
    {
        public ItemData data;
        public bool isCollected;
        public int amountCollected;
        public int amountRequired;
    }

    public Dictionary<ItemData, Vector2Int> questProgress = new Dictionary<ItemData, Vector2Int>();

    public void GenerateDailyQuest()
    {
        currentRequiredItems.Clear();
        questProgress.Clear();

        List<ItemData> pool = new List<ItemData>(questData.possibleItems);

        for (int i = 0; i < questData.itemsToRequest; i++)
        {
            if (pool.Count == 0) break;
            int rand = Random.Range(0, pool.Count);
            ItemData selected = pool[rand];

            int goal = Random.Range(1, 4);

            currentRequiredItems.Add(selected);
            questProgress.Add(selected, new Vector2Int(0, goal));
            pool.RemoveAt(rand);
        }
        OnQuestUpdated?.Invoke();
    }

    private void CheckItemAgainstQuest(GameObject item)
    {
        ItemData pickedData = item.GetComponent<ItemBehavior>().ItemData;

        if (questProgress.ContainsKey(pickedData))
        {
            Vector2Int progress = questProgress[pickedData];

            if (progress.x < progress.y)
            {
                progress.x++;
                questProgress[pickedData] = progress;
                Debug.Log($"{pickedData.ItemName} Progress: {progress.x}/{progress.y}");
                OnQuestUpdated?.Invoke();
            }
        }
    }

    public List<QuestItemStatus> GetQuestStatus()
    {
        List<QuestItemStatus> statusList = new List<QuestItemStatus>();
        foreach (var kvp in collectionStatus)
        {
            statusList.Add(new QuestItemStatus { data = kvp.Key, isCollected = kvp.Value > 0 });
        }
        return statusList;
    }
}
