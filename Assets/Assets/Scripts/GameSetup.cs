using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public bool player;
    public GameObject playerA, playerB;
    public GameObject activePlayer;

    [Header("Setup Components")]
    public CameraControl cam;
    public UI_Map menu;

    void Awake()
    {
        //Find Players
        playerA = GameObject.Find("Player A");
        playerB = GameObject.Find("Player B");

        //Player
        GameObject target = (player) ? playerB : playerA;

        //Get Components
        cam = Camera.main.GetComponent<CameraControl>();
        menu = GameObject.Find("UI/Map").GetComponent<UI_Map>();

        //Setup Components
        menu.Setup(target.GetComponent<PlayerCore>());

        //Mark as active
        activePlayer = target;
    }	
}