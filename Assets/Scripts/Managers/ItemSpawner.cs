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
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] float spawnRadius = 0.5f;

    void Start()
    {
        SpawnItems();
    }

    public void SpawnItems()
    {
        UnityEngine.Tilemaps.Tilemap tilemap = spawnArea.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        if (tilemap == null) { Debug.LogError("SpawnArea must be a Tilemap!"); return; }

        BoundsInt bounds = tilemap.cellBounds;
        int spawnedCount = 0;
        int maxAttempts = totalItems * 50;
        int attempts = 0;

        int totalWeight = 0;
        foreach (var item in itemsToSpawn) totalWeight += item.weight;

        while (spawnedCount < totalItems && attempts < maxAttempts)
        {
            attempts++;

            int x = Random.Range(bounds.xMin, bounds.xMax);
            int y = Random.Range(bounds.yMin, bounds.yMax);
            Vector3Int cellPos = new Vector3Int(x, y, 0);

            if (tilemap.HasTile(cellPos))
            {
                bool isEdge = !tilemap.HasTile(cellPos + Vector3Int.up) ||
                              !tilemap.HasTile(cellPos + Vector3Int.down) ||
                              !tilemap.HasTile(cellPos + Vector3Int.left) ||
                              !tilemap.HasTile(cellPos + Vector3Int.right);

                if (!isEdge)
                {
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);

                    if (!Physics2D.OverlapCircle(worldPos, spawnRadius, itemLayer))
                    {
                        GameObject selectedPrefab = GetWeightedItem(totalWeight);
                        if (selectedPrefab != null)
                        {
                            Instantiate(selectedPrefab, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.identity);
                            spawnedCount++;
                        }
                    }
                }
            }
        }
        Debug.Log($"Tilemap Spawner: {spawnedCount} items in {attempts} attempts.");
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnArea.bounds.center, spawnArea.bounds.size);
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
            GameObject[] oldItems = GameObject.FindGameObjectsWithTag("Herb");
            foreach (GameObject item in oldItems)
            {
                Destroy(item);
            }

            SpawnItems();
        }
    }
}
