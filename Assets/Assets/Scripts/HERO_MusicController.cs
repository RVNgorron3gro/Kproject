using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HERO_MusicController : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] regionMusic = new AudioClip[12];

    public bool layeredMode;

    void Start()
    {
        source = GetComponent<AudioSource>();    
    }

    public void UpdateTrack(string regionName)
    {
        UpdateTrack(Helper.ConvertRegionNameToID(regionName));
    }

    public void UpdateTrack(int regionID)
    {
        /*
        float time = source.time;
        source.clip = regionMusic[regionID];
        if (layeredMode)
            source.time = time;
        source.Play();
        */
    }
}
