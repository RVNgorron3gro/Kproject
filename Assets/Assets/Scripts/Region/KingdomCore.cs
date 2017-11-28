using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingdomCore : MonoBehaviour
{
    [Header("Stats")]
    public int sovereignty;
    public int goldies;
    public int fishingLoyalty;
    public int frozenLoyalty;
    public List<string> tradition;
    public int buildingCount;

    public PlayerCore playerCore;
    public string playerName;
    /*
    void Start()
    {
        sovereignty = 1;
        goldies = 0;
        fishingLoyalty = 0;
        frozenLoyalty = 0;
        //we set player and starting loyalty based on which kingdom we are
        if (gameObject.name == "Kingdom A")
        {
            playerCore = Helper.GetPlayerA().GetComponent<PlayerCore>();
            playerName = playerCore.NameTemp;
            fishingLoyalty = 1;
        } else {
            playerCore = Helper.GetPlayerB().GetComponent<PlayerCore>();
            playerName = playerCore.NameTemp;
            frozenLoyalty = 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.V))
        {
            Debug.Log("z has been released");
            UpdateTraditions();
        }
        /*
        if (Input.GetKeyUp(KeyCode.X))
        {
            Debug.Log("x has been released");
            Debug.Log("i'm kingdom " + gameObject.name);
            Debug.Log(playerName);
        }
        */

    //}

    /*
        public void UpdateTraditions()
        {
            List<RegionCore> regions = Helper.GetAllRegionCores();
            Debug.Log(regions.Count);

            for (int count = 0; count < regions.Count; count++)
            {
                Debug.Log(count);
                if (regions[count].owner == playerName)
                {
                    if (!tradition.Contains(regions[count].tradition))
                    {
                        tradition.Add(regions[count].tradition);
                    }
                }
                else
                {
                    if (tradition.Contains(regions[count].tradition))
                    {
                        tradition.Remove(regions[count].tradition);
                    }
                }
            }
        }
        */
}
