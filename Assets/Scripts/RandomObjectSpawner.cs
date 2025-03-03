using UnityEngine;
using System.Collections;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // Assign a sphere prefab in the Inspector
    public int minObjects = 15;
    public int maxObjects = 20;
    public float minSize = 0.1f;
    public float maxSize = 1f;

    private ProceduralTerrain terrain;

    void Start()
    {
        terrain = GetComponent<ProceduralTerrain>();

        if (terrain == null)
        {
            Debug.LogError("No ProceduralTerrain script found!");
            return;
        }

        if (objectPrefab == null)
        {
            Debug.LogError("Assign a prefab in the Inspector!");
            return;
        }

        // Wait for the terrain to fully generate
        StartCoroutine(SpawnObjectsAfterDelay());
    }

    IEnumerator SpawnObjectsAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // Short delay to ensure mesh is generated

        int objectCount = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 spawnPosition = GetRandomTerrainPosition();
            GameObject newObj = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

            float randomScale = Random.Range(minSize, maxSize);
            newObj.transform.localScale = Vector3.one * randomScale;
        }
    }

    Vector3 GetRandomTerrainPosition()
    {
        if (terrain == null) return Vector3.zero;

        int width = terrain.width;
        int depth = terrain.depth;
        Vector3[] vertices = terrain.GetComponent<MeshFilter>().mesh.vertices;

        // Pick a random (x, z) position within the terrain bounds
        int randomX = Random.Range(0, width);
        int randomZ = Random.Range(0, depth);

        // Find the corresponding vertex to get the terrain height
        int vertexIndex = randomZ * (width + 1) + randomX;
        float terrainHeight = vertices[vertexIndex].y;

        return new Vector3(randomX, terrainHeight, randomZ);
    }
}
