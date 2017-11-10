using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    [Range(1f, 1024f)]public int width, height;
    public float scale;
    public int octaves;
    [Range(0f,1f)]public float persistence;
    public float meshHeightScale;
    public AnimationCurve meshScaleCurve;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoGenerate;

    public TerrainType[] regions;

    void OnValidate()
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }
    
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, octaves, scale, persistence, lacunarity, offset);
        Color[] colourMap = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int r = 0; r < regions.Length; r++)
                {
                    if (currentHeight <= regions[r].height)
                    {
                        colourMap[y * width + x] = regions[r].colour;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColourMap)
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, width, height));
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightScale, meshScaleCurve), 
                TextureGenerator.TextureFromColourMap(colourMap, width, height));
        }
    }
}
