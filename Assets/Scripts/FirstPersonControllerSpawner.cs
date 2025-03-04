using UnityEngine;
using System.Collections;

public class FirstPersonControllerSpawner : MonoBehaviour
{
    public GameObject firstPersonControllerPrefab; // Assign your FPCC prefab in the Inspector
    public float spawnDelay = 0.1f; // Delay to ensure terrain generation completes

    private ProceduralTerrain terrain;

    void Start()
    {
        terrain = FindObjectOfType<ProceduralTerrain>();

        if (terrain == null)
        {
            Debug.LogError("ProceduralTerrain script not found in the scene!");
            return;
        }

        if (firstPersonControllerPrefab == null)
        {
            Debug.LogError("Assign the First Person Controller Prefab in the Inspector!");
            return;
        }

        StartCoroutine(SpawnFirstPersonControllerAfterDelay());
    }

    IEnumerator SpawnFirstPersonControllerAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay); // Wait for the specified delay

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject fpController = Instantiate(firstPersonControllerPrefab, spawnPosition, Quaternion.identity);
        fpController.name = "FirstPersonController";
    }

    Vector3 GetSpawnPosition()
    {
        int width = terrain.width;
        int depth = terrain.depth;
        Vector3[] vertices = terrain.GetComponent<MeshFilter>().mesh.vertices;

        // Calculate the center indices
        int centerX = width / 2;
        int centerZ = depth / 2;

        // Calculate the vertex index for the center
        int vertexIndex = centerZ * (width + 1) + centerX;
        float terrainHeight = vertices[vertexIndex].y;

        return new Vector3(centerX, terrainHeight + 1.0f, centerZ); // Offset by 1 unit above the terrain
    }
}
