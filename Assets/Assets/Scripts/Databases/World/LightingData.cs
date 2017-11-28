using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Data", menuName = "Data/World/Lighting", order = 0)]
public class LightingData : ScriptableObject
{
    public int dayTurns;

    [Range(0, 2)]
    public float[] sunIntensity;
    public Color[] sunColor;
    [Range(0, 1)]
    public float[] ambientIntensity;

    void OnEnable()
    {
        if ((sunIntensity.Length | sunColor.Length | ambientIntensity.Length) != dayTurns)
        {
            ConstructLightingData();
        }   
    }

    public void ConstructLightingData()
    {
        sunIntensity = new float[dayTurns];
        sunColor = new Color[dayTurns];
        ambientIntensity = new float[dayTurns];
    }
}