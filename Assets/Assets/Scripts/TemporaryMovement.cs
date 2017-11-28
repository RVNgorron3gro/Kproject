using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TemporaryMovement : MonoBehaviour
{
    NavMeshAgent player;

    void Start()
    {
        player = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(interactionRay, out hitInfo, Mathf.Infinity))
            {
                player.destination = hitInfo.point;
            }
        }
    }
}
