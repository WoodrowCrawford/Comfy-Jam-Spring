using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public static bool HasCollectedAllQuestItems;

    private int weeklyCollectedTotal = 0;
    private int weeklyRequiredTotal = 0;

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
        HasCollectedAllQuestItems = false;
        currentRequiredItems.Clear();
        questProgress.Clear();

        // Add today's required items to weekly total
        foreach (var item in questData.possibleItems)
        {
            // Only add if this item is selected for today's quest
            if (!questProgress.ContainsKey(item)) continue;
            weeklyRequiredTotal += questProgress[item].y;
        }

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
                weeklyCollectedTotal++;
                HasCollectedAllQuestItems = AreAllQuestItemsCollected();
                Debug.Log($"{pickedData.ItemName} Progress: {progress.x}/{progress.y}");
                OnQuestUpdated?.Invoke();
            }
        }
    }

    public static float GetWeeklyQuestCompletionPercentage()
    {
        if (Instance == null || Instance.weeklyRequiredTotal == 0) return 0f;
        return (float)Instance.weeklyCollectedTotal / Instance.weeklyRequiredTotal;
    }

    public static void ResetWeeklyQuestProgress()
    {
        if (Instance == null) return;
        Instance.weeklyCollectedTotal = 0;
        Instance.weeklyRequiredTotal = 0;
    }

    private bool AreAllQuestItemsCollected()
    {
        if (questProgress.Count == 0)
        {
            return false;
        }

        foreach (var progress in questProgress.Values)
        {
            if (progress.x < progress.y)
            {
                return false;
            }
        }

        return true;
    }

    public float GetDailyQuestCompletionPercentage()
    {
        if (questProgress.Count == 0)
        {
            return 0f;
        }

        float totalCollected = 0f;
        float totalRequired = 0f;

        foreach (var progress in questProgress.Values)
        {
            totalCollected += progress.x;
            totalRequired += progress.y;
        }

        if (totalRequired <= 0f)
        {
            return 0f;
        }

        return totalCollected / totalRequired;
    }

    public void ResetDailyQuestProgress()
    {
        List<ItemData> items = new List<ItemData>(questProgress.Keys);

        foreach (ItemData item in items)
        {
            Vector2Int progress = questProgress[item];
            questProgress[item] = new Vector2Int(0, progress.y);
        }

        HasCollectedAllQuestItems = false;
        OnQuestUpdated?.Invoke();
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
