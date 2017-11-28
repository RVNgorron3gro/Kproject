using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTSControl : MonoBehaviour
{
    public Camera mainCamera;

    //True = Menu, False = In Game
    public bool mode;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {

    }

}
