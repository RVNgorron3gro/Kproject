using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionDetection : MonoBehaviour
{
    public GameObject edges;

    void Start()
    {
        edges = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerA")
        {
            Debug.Log("Player A has moved to " + transform.parent.name);
        }

        if (other.tag == "PlayerB")
        {
            Debug.Log("Player B has moved to " + transform.parent.name);
        }
    }

    public void ConfigureWallColliders(bool status)
    {
        if (status)
        {
            edges.SetActive(true);
        }
        else
        {
            edges.SetActive(false);
        }
    }
}
