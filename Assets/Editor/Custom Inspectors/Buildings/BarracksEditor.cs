using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingCore.Barracks))]
public class BarracksEditor : Editor
{
    BuildingCore.Barracks barracks;

    void OnEnable()
    {
        barracks = (BuildingCore.Barracks)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Assault (1)"))
        {
            barracks.QueueNewUnit(TroopCore.Type.Assault);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Ambush (1)"))
        {
            barracks.QueueNewUnit(TroopCore.Type.Ambush);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Defender (1)"))
        {
            barracks.QueueNewUnit(TroopCore.Type.Defender);
        }

        EditorGUILayout.EndHorizontal();
    }
}