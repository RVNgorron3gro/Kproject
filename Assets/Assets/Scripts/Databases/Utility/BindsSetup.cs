using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Custom Keys Setup", menuName = "Data/Utility/New KeyBind Setup", order = 4)]
[System.Serializable]
public class BindsSetup : ScriptableObject
{
    public List<Binds.Binding> contained;
}