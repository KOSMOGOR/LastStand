using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
[CanEditMultipleObjects]
public class GridManagerEditor : Editor
{
    GridManager t;

    void OnEnable() {
        t = target as GridManager;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Label($"Tiles count: {t.tiles.Count}");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate")) t.GenerateGrid();
        if (GUILayout.Button("Remove")) t.RemoveGrid();
        GUILayout.EndHorizontal();
    }
}
