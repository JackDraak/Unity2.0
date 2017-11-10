using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [Range(1f, 1024f)]public int width, height;
    public float scale;
    public int octaves;
    [Range(0f,1f)]public float persistence;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoGenerate;

    void OnValidate()
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, octaves, scale, persistence, lacunarity, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
}
