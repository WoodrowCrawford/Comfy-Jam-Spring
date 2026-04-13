using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[System.Serializable]
public class SpawnableItem
{
    public GameObject prefab;
    [UnityEngine.Range(0, 100)] public int weight;
}

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] List<SpawnableItem> itemsToSpawn = new List<SpawnableItem>();
    [SerializeField] int totalItems = 10;
    [SerializeField] Collider2D spawnArea;

    void Start()
    {
        SpawnItems();
    }

    public void SpawnItems()
    {
        Bounds bounds = spawnArea.bounds;
        float cellSize = 2f;
        int columns = Mathf.FloorToInt(bounds.size.x / cellSize);
        int rows = Mathf.FloorToInt(bounds.size.y / cellSize);

        List<Vector2> gridPoints = new List<Vector2>();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                float xPos = bounds.min.x + (x * cellSize) + (cellSize / 2);
                float yPos = bounds.min.y + (y * cellSize) + (cellSize / 2);

                xPos += Random.Range(-cellSize / 4, cellSize / 4);
                yPos += Random.Range(-cellSize / 4, cellSize / 4);

                gridPoints.Add(new Vector2(xPos, yPos));
            }
        }

        for (int i = 0; i < gridPoints.Count; i++)
        {
            Vector2 temp = gridPoints[i];
            int randomIndex = Random.Range(i, gridPoints.Count);
            gridPoints[i] = gridPoints[randomIndex];
            gridPoints[randomIndex] = temp;
        }

        int totalWeight = 0;
        foreach (var item in itemsToSpawn) totalWeight += item.weight;

        int limit = Mathf.Min(totalItems, gridPoints.Count);
        for (int i = 0; i < limit; i++)
        {
            GameObject selectedPrefab = GetWeightedItem(totalWeight);
            Instantiate(selectedPrefab, gridPoints[i], Quaternion.identity);
        }
    }

    GameObject GetWeightedItem(int weight)
    {
        int roll = Random.Range(0, weight);
        int cursor = 0;

        foreach(var item in itemsToSpawn)
        {
            cursor += item.weight;
            if (roll < cursor) return item.prefab;
        }
        return itemsToSpawn[0].prefab;
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Restart Key Pressed!");
            GameObject[] oldItems = GameObject.FindGameObjectsWithTag("Herb");
            foreach (GameObject item in oldItems)
            {
                Destroy(item);
            }

            SpawnItems();
        }
    }
}
