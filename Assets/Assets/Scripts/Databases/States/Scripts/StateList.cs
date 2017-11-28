using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateList : MonoBehaviour
{
    [HideInInspector]
    public static StateList i;
    public List<State> contained;

    void Awake()
    {
        i = this;
    }
}