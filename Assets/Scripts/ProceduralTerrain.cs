using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProceduralTerrain : MonoBehaviour
{
    public int width = 10;
    public int depth = 10;
    public float maxHeight = 5f;
    public float scale = 2f;

    public Material terrainMaterial; // Custom Shader Material for Blending
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;
    private Color[] colors;

    void Start()
    {
        GenerateTerrain();
        ApplyMaterial();
    }

    void GenerateTerrain()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateVertices();
        CreateTriangles();
        CreateUVs();
        CreateVertexColors(); // Assigns vertex colors for blending
        UpdateMesh();
    }

    void CreateVertices()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];

        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * scale * 0.1f, z * scale * 0.1f) * maxHeight;
                vertices[z * (width + 1) + x] = new Vector3(x, y, z);
            }
        }
    }

    void CreateTriangles()
    {
        triangles = new int[width * depth * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void CreateUVs()
    {
        uvs = new Vector2[vertices.Length];

        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                uvs[z * (width + 1) + x] = new Vector2((float)x / width, (float)z / depth);
            }
        }
    }

    void CreateVertexColors()
    {
        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            float height = vertices[i].y / maxHeight;

            // Assign color-based blending:
            if (height < 0.2f)
                colors[i] = Color.blue;  // Water level
            else if (height < 0.4f)
                colors[i] = Color.green; // Grass
            else if (height < 0.6f)
                colors[i] = Color.yellow; // Dirt
            else if (height < 0.8f)
                colors[i] = Color.grey;  // Rock
            else
                colors[i] = Color.white; // Snow
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors; // Assign vertex colors for blending
        mesh.RecalculateNormals();

        // Assign MeshCollider for collision detection
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }
    }

    void ApplyMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (terrainMaterial != null)
        {
            renderer.material = terrainMaterial; // Apply blending shader
        }
        else
        {
            Debug.LogWarning("No terrain material assigned. Please assign one in the Inspector.");
        }
    }
}
