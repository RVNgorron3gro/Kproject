using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponCore))]
public class WeaponCoreEditor : Editor
{
    /*
    void OnSceneGUI()
    {
        WeaponCore wc = (WeaponCore)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(wc.transform.position, Vector3.up, Vector3.forward, 360, 1.25f);
        Vector3 viewAngleA = wc.DirFromAngle(-wc.activeBlockAngle / 2, false);
        Vector3 viewAngleB = wc.DirFromAngle(wc.activeBlockAngle / 2, false);
        Handles.DrawLine(wc.transform.position, wc.transform.position + viewAngleA * 1.25f);
        Handles.DrawLine(wc.transform.position, wc.transform.position + viewAngleB * 1.25f);
    }
    */
}