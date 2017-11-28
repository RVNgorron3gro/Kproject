using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController : NetworkBehaviour
{
    public static AudioController i;
    public AudioSource localSource;
    public GameObject spawnSource;

    void Awake()
    {
        if (!i)
        {
            i = this;
        }
        else
        {
            Debug.LogError(this + " : THERE ARE MULTIPLE INSTANCES OF THIS SCRIPT");
            Destroy(this);
        }
    }

    void Start()
    {
        localSource = GetComponent<AudioSource>();
    }

    public void Play2DSound(AudioClip clip, float volume, float pitchMin = 1, float pitchMax = 1)
    {
        localSource.clip = clip;
        localSource.volume = volume;
        localSource.pitch = Random.Range(pitchMin, pitchMax);
        localSource.Play();
    }

    //[Command]
    public void CmdPlayPublic2DSound(AudioClip clip, float volume, float pitchMin, float pitchMax)
    {

    }

    public void Play3DSound(AudioClip clip, Vector3 position, float volume, float distanceMin, float distanceMax, float pitchMin = 1, float pitchMax = 1)
    {

    }


    //[Command]
    public void CmdPlayPublic3DSound(AudioClip clip, Vector3 position, float volume, float distanceMin, float distanceMax, float pitchMin, float pitchMax)
    {

    }

    [Command]
    public void CmdPlay3DItemSound(string region, int ID, Defs.ItemSound target, Vector3 position, float volume, float distanceMin, float distanceMax, float pitchMin, float pitchMax)
    {
        RpcPlay3DItemSound(region, ID, target, position, volume, distanceMin, distanceMax, pitchMin, pitchMax);
    }

    [ClientRpc]
    public void RpcPlay3DItemSound(string region, int ID, Defs.ItemSound target, Vector3 position, float volume, float distanceMin, float distanceMax, float pitchMin, float pitchMax)
    {
        bool criteria = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int count = 0; count < players.Length; count++)
        {
            if (players[count].GetComponent<Player>().isLocalPlayer)
            {
                if (players[count].GetComponent<PlayerCore>().currentLocation == region)
                {
                    criteria = true;
                }
                break;
            }
        }

        if (criteria)
        {
            Item targetItem = MasterListDatabase.i.FetchItem(ID);
            AudioClip[] clips;
            AudioClip targetClip;
            switch (target)
            {
                case Defs.ItemSound.WindupSound:
                    clips = targetItem.WindupSound;
                    break;
                case Defs.ItemSound.StrikeSound:
                    clips = targetItem.StrikeSound;
                    break;
                case Defs.ItemSound.HitSound:
                    clips = targetItem.HitSound;
                    break;
                case Defs.ItemSound.FlightSound:
                    clips = targetItem.FlightSound;
                    break;
                case Defs.ItemSound.BlockSound:
                    clips = targetItem.BlockSound;
                    break;
                default:
                    clips = null;
                    Debug.Log("No Sounds Defined!");
                    break;
            }

            if (clips != null && clips.Length != 0)
            {
                GameObject spawned = Instantiate(spawnSource, position, new Quaternion());
                AudioSource source = spawned.GetComponent<AudioSource>();

                targetClip = clips[Random.Range(0, clips.Length)];
                source.clip = targetClip;
                source.volume = volume;
                source.minDistance = distanceMin;
                source.maxDistance = distanceMax;
                source.pitch = Random.Range(pitchMin, pitchMax);
                source.Play();

                //Destroy When Complete
                Destroy(spawned, targetClip.length + 0.1f);
            }
        }
    }
}