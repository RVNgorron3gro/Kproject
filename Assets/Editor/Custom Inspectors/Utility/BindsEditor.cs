using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BindsSetup))]
public class CustomKeysCoreEditor : Editor
{
    BindsSetup instance;

    void OnEnable()
    {
        instance = (BindsSetup)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Bind"))
        {
            instance.contained.Add(new Binds.Binding());
        }
        if(GUILayout.Button("Delete Last Bind"))
        {
            instance.contained.RemoveAt(instance.contained.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
    }
}