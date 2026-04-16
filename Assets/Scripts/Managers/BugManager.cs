using System.Collections.Generic;
using UnityEngine;

public class BugManager : MonoBehaviour
{
    [SerializeField] GameObject[] bugPrefabs;
    [SerializeField] int maxBugsOnScreen = 3;
    public static float spawnBuffer = 2f;

    List<GameObject> activeBugs = new List<GameObject>();
    Camera cam;

    void Start()
    {
        cam = Camera.main;

        for (int i = 0; i < maxBugsOnScreen; i++)
        {
            SpawnBug(true);
        }
    }

    void Update()
    {
        activeBugs.RemoveAll(item => item == null);

        if (activeBugs.Count < maxBugsOnScreen)
        {
            SpawnBug(false);
        }
    }

    void SpawnBug(bool isInitial)
    {
        if (bugPrefabs == null || bugPrefabs.Length == 0) return;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;
        Vector3 camPos = cam.transform.position;

        float spawnX, spawnY;

        if (isInitial)
        {
            spawnX = Random.Range(camPos.x - width, camPos.x + width);
            spawnY = Random.Range(camPos.y - height, camPos.y + height);
        }
        else
        {
            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0:
                    spawnX = Random.Range(camPos.x - width, camPos.x + width);
                    spawnY = camPos.y + height + 1f;
                    break;
                case 1:
                    spawnX = Random.Range(camPos.x - width, camPos.x + width);
                    spawnY = camPos.y - height - spawnBuffer;
                    break;
                case 2:
                    spawnX = camPos.x - width - spawnBuffer;
                    spawnY = Random.Range(camPos.y - height, camPos.y + height);
                    break;
                default:
                    spawnX = camPos.x + width + spawnBuffer;
                    spawnY = Random.Range(camPos.y - height, camPos.y + height);
                    break;
            }
        }

        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        Vector2 targetPos = (Vector2)camPos + new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        Vector2 directionVector = (targetPos - spawnPos).normalized;

        GameObject randomBugPrefab = bugPrefabs[Random.Range(0, bugPrefabs.Length)];
        GameObject newBug = Instantiate(randomBugPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);

        activeBugs.Add(newBug);
        newBug.GetComponent<BugAI>().SetStartingDirection(directionVector);
    }
}