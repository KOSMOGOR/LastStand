using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScriptableObjectAssetCreator
{
    [MenuItem ("Assets/Create ScriptableObject")]
    public static void Create ()
    {
        var script = Selection.activeObject as MonoScript;
        var type = script.GetClass();
        var scriptableObject = ScriptableObject.CreateInstance(type);
        var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
        var currentName = Selection.activeObject.name;
        int i = 0;
        while (i <= 10) {
            currentName = Selection.activeObject.name + (i == 0 ? "" : i.ToString());
            if (AssetDatabase.AssetPathExists($"{path}/{currentName}.asset")) i += 1;
            else break;
        } 
        AssetDatabase.CreateAsset(scriptableObject, $"{path}/{currentName}.asset");
    }

    [MenuItem ("Assets/Create ScriptableObject", true)]
    public static bool ValidateCreate ()
    {
        var script = Selection.activeObject as MonoScript;
        return script != null && script.GetClass ().IsSubclassOf (typeof(ScriptableObject));
    }
}