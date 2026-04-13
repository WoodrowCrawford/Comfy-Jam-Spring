using System.Collections.Generic;
using UnityEngine;

public class BugManager : MonoBehaviour
{
    [SerializeField] GameObject[] bugPrefabs;
    [SerializeField] int maxBugsOnScreen = 3;
    public static float spawnBuffer = 4f;

    List<GameObject> activeBugs = new List<GameObject>();
    float screenBoundX;
    float screenBoundY;

    void Start()
    {
        screenBoundX = Camera.main.orthographicSize * Camera.main.aspect;
        screenBoundY = Camera.main.orthographicSize;

        for (int i = 0; i < maxBugsOnScreen; i++)
        {
            SpawnBug(true);
        }

        InvokeRepeating("CleanAndSpawnBugs", 0.5f, 0.5f);
    }

    void CleanAndSpawnBugs()
    {
        activeBugs.RemoveAll(item => item == null);
        if (activeBugs.Count < maxBugsOnScreen)
        {
            SpawnBug(false);
        }
    }

    void Update()
    {
        for (int i = activeBugs.Count - 1; i >= 0; i--)
        {
            if (activeBugs[i] == null)
            {
                activeBugs.RemoveAt(i);
            }
        }

        if (activeBugs.Count < maxBugsOnScreen)
        {
            SpawnBug(false);
        }
    }

    void SpawnBug(bool isInitial)
    {
        float buffer = 4f;

        if (bugPrefabs == null || bugPrefabs.Length == 0) return;

        GameObject randomBugPrefab = bugPrefabs[UnityEngine.Random.Range(0, bugPrefabs.Length)];

        float spawnX = UnityEngine.Random.Range(-screenBoundX, screenBoundX);
        float spawnY = UnityEngine.Random.Range(-screenBoundY, screenBoundY);

        if (!isInitial)
        {
            int edge = UnityEngine.Random.Range(0, 4);
            switch (edge)
            {
                case 0: spawnX = screenBoundX + spawnBuffer; break;
                case 1: spawnX = -screenBoundX - spawnBuffer; break;
                case 2: spawnY = screenBoundY + spawnBuffer; break;
                case 3: spawnY = -screenBoundY - spawnBuffer; break;
            }
        }

        Vector2 spawnPos = new Vector2(spawnX, spawnY);
        Vector2 targetPos = new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
        Vector2 directionVector = (targetPos - spawnPos).normalized;

        GameObject newBug = Instantiate(randomBugPrefab, spawnPos, Quaternion.identity);
        activeBugs.Add(newBug);

        newBug.GetComponent<BugAI>().SetStartingDirection(directionVector);
    }
}