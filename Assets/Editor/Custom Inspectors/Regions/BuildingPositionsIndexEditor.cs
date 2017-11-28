using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingPositionsIndex))]
public class BuildingPositionsIndexEditor : Editor
{
    BuildingPositionsIndex bpi;

    void OnEnable()
    {
        bpi = (BuildingPositionsIndex)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Layout"))
        {
            bpi.CreateNewEntry();
        }
        if(GUILayout.Button("Restore Layout"))
        {
            bpi.Restore();
        }
        EditorGUILayout.EndHorizontal();
    }
}