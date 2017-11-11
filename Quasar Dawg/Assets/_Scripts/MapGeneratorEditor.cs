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
            if (generator.autoGenerate) generator.DrawMapInEditor();
        }
        if (GUILayout.Button("Re-generate")) generator.DrawMapInEditor();
    }
}
