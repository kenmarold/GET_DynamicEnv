using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObjectCollection
    {
        public string name;
        public List<GameObject> prefabs = new List<GameObject>();
        public int minSpawnCount;
        public int maxSpawnCount;
    }

    public ObjectCollection rocksCollection = new ObjectCollection { name = "Rocks" };
    public ObjectCollection plantsCollection = new ObjectCollection { name = "Plants" };
    public ObjectCollection treesCollection = new ObjectCollection { name = "Trees" };
    public ObjectCollection dustCollection = new ObjectCollection { name = "Dust" }; // New Dust Collection

    public float spawnDelay = 0.1f;
    public float overlapCheckRadius = 0.5f;
    public int maxSpawnAttempts = 100;

    private ProceduralTerrain terrain;
    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        terrain = GetComponent<ProceduralTerrain>();

        if (terrain == null)
        {
            Debug.LogError("ProceduralTerrain script not found!");
            return;
        }

        StartCoroutine(SpawnAllObjectsAfterDelay());
    }

    IEnumerator SpawnAllObjectsAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);

        SpawnObjects(rocksCollection);
        SpawnObjects(plantsCollection);
        SpawnObjects(treesCollection);
        SpawnDust(dustCollection); // Call to spawn dust
    }

    void SpawnObjects(ObjectCollection collection)
    {
        int spawnCount = Random.Range(collection.minSpawnCount, collection.maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = collection.prefabs[Random.Range(0, collection.prefabs.Count)];
            Vector3 spawnPosition;

            int attempts = 0;
            do
            {
                spawnPosition = GetRandomTerrainPosition();
                attempts++;
            } while (IsOverlapping(spawnPosition) && attempts < maxSpawnAttempts);

            if (attempts >= maxSpawnAttempts)
            {
                Debug.LogWarning($"Could not find non-overlapping position for {prefab.name} after {maxSpawnAttempts} attempts.");
                continue;
            }

            Instantiate(prefab, spawnPosition, Quaternion.identity);
            usedPositions.Add(spawnPosition);
        }
    }

    void SpawnDust(ObjectCollection collection)
    {
        int spawnCount = Random.Range(collection.minSpawnCount, collection.maxSpawnCount + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject dustPrefab = collection.prefabs[Random.Range(0, collection.prefabs.Count)];
            Vector3 spawnPosition;

            int attempts = 0;
            do
            {
                spawnPosition = GetRandomTerrainPosition();
                attempts++;
            } while (IsOverlapping(spawnPosition) && attempts < maxSpawnAttempts);

            if (attempts >= maxSpawnAttempts)
            {
                Debug.LogWarning($"Could not find non-overlapping position for {dustPrefab.name} after {maxSpawnAttempts} attempts.");
                continue;
            }

            Instantiate(dustPrefab, spawnPosition, Quaternion.identity);
            usedPositions.Add(spawnPosition);
        }
    }

    Vector3 GetRandomTerrainPosition()
    {
        int width = terrain.width;
        int depth = terrain.depth;
        Vector3[] vertices = terrain.GetComponent<MeshFilter>().mesh.vertices;

        int randomX = Random.Range(0, width);
        int randomZ = Random.Range(0, depth);

        int vertexIndex = randomZ * (width + 1) + randomX;
        float terrainHeight = vertices[vertexIndex].y;

        return new Vector3(randomX, terrainHeight, randomZ);
    }

    bool IsOverlapping(Vector3 position)
    {
        foreach (var usedPos in usedPositions)
        {
            if (Vector3.Distance(position, usedPos) < overlapCheckRadius)
            {
                return true;
            }
        }
        return false;
    }
}
