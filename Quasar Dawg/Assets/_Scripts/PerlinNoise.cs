using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    [Header("Change Me!")][Tooltip("Density of noise.")]public float scale = 20f;
    [Space(10)]
    public int width = 256;
    public int height = 256;
    [Space(10)]
    public float offsetX;
    public float offsetY;
    Renderer renderer;

    void Start()
    {
        bool pass;
        pass = (renderer = GetComponent<Renderer>());
        if (!pass) { Debug.Log("Start() get renderer FAIL."); }

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);
    }

    void Update()
    {
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color colour = CalculateColour(x, y);
                texture.SetPixel(x, y, colour);
            }
        }
        texture.Apply();
        return texture;
    }

    Color CalculateColour(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
