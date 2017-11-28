using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [Header("State")]
    public bool active;
    public float cooldown;

    [Header("Materials")]
    public Material targetMaterial;

    void Start()
    {
        targetMaterial = GetComponent<Terrain>().materialTemplate;
    }

    void Update()
    {
        if (cooldown != 0)
        {
            if (active)
            {
                targetMaterial.color = Color.Lerp(targetMaterial.color, Color.white, 0.05f);
            }
            else
            {
                targetMaterial.color = Color.Lerp(targetMaterial.color, Color.gray, 0.05f);
            }
        }
        else
        {
            cooldown = Mathf.Clamp(cooldown - Time.deltaTime, 0, 1);
        }
    }

    public void ChangeVisiblity(PlayerCore player)
    {
        if (player.currentLocation == gameObject.name)
        {
            active = true;
        }
        else
        {
            active = false;
        }
        cooldown = 1;
    }
}
