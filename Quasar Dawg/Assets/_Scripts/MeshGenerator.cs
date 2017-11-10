using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        int meshDetailIncrement = (levelOfDetail == 0)?1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshDetailIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for (int x = 0; x < width; x += meshDetailIncrement)
        {
            for (int y = 0; y < height; y += meshDetailIncrement)
            {
                meshData.vertices[vertexIndex] = 
                    new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightScale, topLeftZ - y);
                meshData.uvs[vertexIndex] = 
                    new Vector2(x / (float)width, y / (float)height);
                if (x < width -1 && y < height -1)
                {
                    // Entering triangles this way led to the mesh being upside down... reversed order.
                    //    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    //    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine, vertexIndex + verticesPerLine + 1);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex + 1, vertexIndex);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    int TriangleIndex;


    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[TriangleIndex] = a;
        triangles[TriangleIndex+1] = b;
        triangles[TriangleIndex+2] = c;
        TriangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
