using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MasterList))]
public class MasterListEditor : Editor
{
    MasterList instance;

    void OnEnable()
    {
        instance = (MasterList)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Current Items: " + instance.c.Count);

        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Item"))
        {
            instance.c.Add(new MasterList.Contained());
        }
        if (GUILayout.Button("Delete Last Item"))
        {
            instance.c.RemoveAt(instance.c.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
    }
}