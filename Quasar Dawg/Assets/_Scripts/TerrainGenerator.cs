using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public int depth = 20;
    public int width = 256;
    public int height = 256;

    public float scale = 10f;
    public float xOffset;
    public float yOffset;
    [Range(-20, 20)]public float speed = 10f;

    Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        xOffset = Random.Range(0f, 9999f);
        yOffset = Random.Range(0f, 9999f);
    }

    void Update()
    {
        xOffset += speed * Time.deltaTime;
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + xOffset;
        float yCoord = (float)y / height * scale + yOffset;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
