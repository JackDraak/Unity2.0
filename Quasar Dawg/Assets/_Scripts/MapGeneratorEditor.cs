using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator generator = (MapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (generator.autoGenerate) generator.GenerateMap();
        }
        if (GUILayout.Button("Re-generate")) generator.GenerateMap();
    }
}
