using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RegionCore : NetworkBehaviour
{
    [Header("Identification")]
    [SyncVar]
    public bool nonCore;
    [SyncVar]
    public int regionID;
    [SyncVar]
    public string regionHandle;

    [Header("Status")]
    public bool Active;
    public bool DeadLock;

    [Header("Buildings")]
    public GameObject building;
    public List<BuildingCore> ownedBuildings;

    [Header("Fog of War")]
    public Material visible;
    public Material fogOfWar;

    [Header("Critters")]
    public List<GameObject> spawnpoints;
    public GameObject critterObject;
    public List<GameObject> ownedCritters;
    //public int naturalMax;
    //max neutrals should be initialized as naturalmax
    public int maxNeutrals;
    //public int currentneutrals = Random.Range(2, 5);
    public int currentNeutrals;
    public int habitableThreshold;
    public int growth;

    [Header("Capture")]
    public List<PlayerCore> playersInRegion;
    [SyncVar]
    public int owner = -1;
    [SyncVar]
    public int capturer = -1;
    public bool habitable;
    public bool claimable;
    public bool claimed;
    public bool uncontested;
    [SyncVar]
    public float captureTimer;

    [Header("Specifics")]
    [SyncVar]
    public string tradition;

    [ServerCallback]
    void Start()
    {
        gameObject.name = regionHandle;
        tradition = regionHandle;
        regionID = transform.GetSiblingIndex();

        //Populate Spawnpoints
        for (int count = 0; count < transform.GetChild(1).childCount; count++)
        {
            spawnpoints.Add(transform.GetChild(1).GetChild(count).gameObject);
        }
        CheckRegionStatus();
    }

    [ServerCallback]
    void Update()
    {
        if (captureTimer != 0 && playersInRegion.Count == 0)
        {
            HandleCaptureDecay();
        }

        if(playersInRegion.Count == 2)
        {
            if (!DeadLock)
                Clock.i.TimeRateModifier(true);
            DeadLock = true;
        }
        else
        {
            if (DeadLock)
                Clock.i.TimeRateModifier(false);
            DeadLock = false;
        }
    }

    [Server]
    public void PassTurn()
    {
        CheckRegionStatus();

        //Check if active
        if (playersInRegion.Count == 0)
        {
            Active = false;
        }
        else
        {
            Active = true;
        }

        //Despawn Critters
        if (!Active)
        {
            for (int count = 0; count < ownedCritters.Count; count++)
            {
                Destroy(ownedCritters[count]);
                NetworkServer.Destroy(ownedCritters[count]);
            }
            ownedCritters.Clear();
        }
    }

    [Server]
    public void CheckRegionStatus()
    {
        if (ownedCritters.Count <= habitableThreshold)
        {
            habitable = true;
        }
        else
        {
            habitable = false;
        }
        GetBuildingsInRegion();
    }

    //Buildings
    [Server]
    public void GetBuildingsInRegion()
    {
        GameObject[] filter = GameObject.FindGameObjectsWithTag("Building");
        List<BuildingCore> buildingList = new List<BuildingCore>();
        for (int count = 0; count < filter.Length; count++)
        {
            if (filter[count].GetComponent<BuildingCore>().masterRegion == regionHandle)
            {
                buildingList.Add(filter[count].GetComponent<BuildingCore>());
            }
        }
        ownedBuildings = buildingList;
    }

    /*
    //Critters
    [Server]
    public List<GameObject> GetCrittersInRegion()
    {
        GameObject[] filter = GameObject.FindGameObjectsWithTag("Critter");
        List<GameObject> critterList = new List<GameObject>();
        for (int count = 0; count < filter.Length; count++)
        {
            if (filter[count].GetComponent<CritterCore>().masterRegion == regionHandle)
            {
                critterList.Add(filter[count]);
            }
        }
        //Debug.Log("REGION CORE: Critter Count (" + critterList.Count + ") in " + regionHandle);
        return critterList;
    }
    */


    #region so much commenting wtf
    /*
    [System.Serializable]
    public class Weather : MonoBehaviour
    {
        [Header("Values")]
        //public int cloudyStat = Random.Range(0, 100);
        public int cloudyStat = 0;
        public string weatherName = "";

        private void Update()
        {
            if (cloudyStat >= 75)
            {
                weatherName = "Foggy";
            }
            else if (cloudyStat >= 40)
            {
                weatherName = "Cloudy";
            }
            else if (cloudyStat >= 0)
            {
                weatherName = "Sunny";
            }
        }
    }
    */

    /*
     * -/-/- population as a different class just in case you prefer it that way -/-/-
    [System.Serializable]
    public class Population : MonoBehaviour
    {
        [Header("Values")]

    }
    */

    /*
    [System.Serializable]
    public class LandStatus2 : MonoBehaviour
    {
        [Header("Values")]
        public int naturalmax = 7;
        //max neutrals should be initialized as naturalmax
        public int maxneutrals = 7;
        //public int currentneutrals = Random.Range(2, 5);
        public int currentneutrals = 0;
        public int habthreshold = 3;
        public int growth = 1;



        public int capcapped = 15;
        public bool charinregion = false;
        public float captimer = 0;
        public string owner = "";
        public bool traditionunlocked = false;

        /// <summary>
        /// calculates the maximum population in the region
        /// </summary>
        void maxPopMthd()
        {
            if (claimed == false)
            {
                maxneutrals = naturalmax;
            }
            else
            {
                maxneutrals = habthreshold;
            }
        }

        /// <summary>
        /// sets the habitability based on the number of creeps within the region
        /// </summary>
        void habitabilityMthd()
        {
            if (currentneutrals > habthreshold)
            {
                habitable = false;
            }
            else
            {
                habitable = true;
            }
        }

        /// <summary>
        /// manages the values related to capturing a region
        /// </summary>
        void capturingZoneMthd()
        {
            if (habitable == true)
            {
                if (captimer < capcapped & charinregion == true)
                //&& our main character is within the region
                //&& is not claimed by someone already
                {
                    //+1 to captimer per second
                    captimer += 1 * Time.deltaTime;
                    if (captimer >= capcapped)
                    {
                    
                        claimable = true;
                        
                         * it should also be defined for who is this land claimable, so claimable = true won't mean your enemy can claim it too
                         
                    }
                }
            }
        }


        /// <summary>
        /// sets the region to claimable if it's captured
        /// </summary>
        void claimabilityMthd()
        {
            if (captimer >= capcapped)
            {
                claimable = true;
            }
        }


        /// <summary>
        /// sets the land as claimed when a building is built
        /// </summary>
        void claimedMthd()
        {
            //if there is a building
            //claimed = true;
            //owner = "player who built the building"
            //else 
        }

        /// <summary>
        /// sets the land as contested when they're claimed for the first time
        /// </summary>
        void turnToContested()
        {
            if (claimed == true && uncontested == true)
            {
                //add 1 sovereignty for the kingdom that captures it
                uncontested = false;
            }
        }

        private void Update()
        {
            habitabilityMthd();
            capturingZoneMthd();
        }
    }

*/
    #endregion

    [Command]
    public void CmdHandleCapture(int playerID)
    {
        if (!nonCore && habitable && !claimable && playerID != owner)
        {
            capturer = playerID;
            uncontested = false;
            captureTimer = Mathf.Clamp(captureTimer + Time.deltaTime, 0, 15);
            RpcUpdateCapTimer(playerID);

            //Captured
            if (captureTimer == 15)
            {
                owner = playerID;
                claimable = true;
                captureTimer = 0;
                RpcUpdateLocationAndTimer(playerID, false);

                //Add 1 Sovereignty
                //GameStatus.players[playerID].
                //player.GetComponent<KingdomCore>().sovereignty++;
            }
        }
    }

    [ClientRpc]
    public void RpcUpdateCapTimer(int playerID)
    {
        if (UI_HUD.i.targetID == playerID)
        {
            UI_HUD.i.UpdateCaptureTimer(captureTimer / 15);
        }
    }

    [ClientRpc]
    public void RpcUpdateLocationAndTimer(int playerID, bool fail)
    {
        if (UI_HUD.i.targetID == playerID)
        {
            UI_HUD.i.PingCaptureTimer(fail);
            UI_HUD.i.UpdateLocation(owner, nonCore, regionHandle, playerID);
        }
    }

    void HandleCaptureDecay()
    {
        captureTimer = Mathf.Clamp(captureTimer - Time.deltaTime, 0, 15);
        capturer = -1;
    }    

    [Server]
    public void SpawnCritters()
    {
        if (ownedCritters.Count == 0)
        {
            List<float> x = new List<float>();
            for (int count = 0; count < spawnpoints.Count; count++)
            {
                x.Add(0);
            }

            for (int count = 0; count < currentNeutrals; count++)
            {
                int point = Random.Range(0, spawnpoints.Count);
                Vector3 pos = spawnpoints[point].transform.position + new Vector3(x[point], 1, 0);
                GameObject newCritter = Instantiate(critterObject, pos, new Quaternion(-90, 0, 0, 90), spawnpoints[point].transform);
                ownedCritters.Add(newCritter);
                NetworkServer.Spawn(newCritter);
                x[point] += 1;
            }
        }
    }

    [Server]
    public void ReportCritterDeath(GameObject critter)
    {
        ownedCritters.Remove(critter);
        CheckRegionStatus();
    }

    [Server]
    public void ConstructBuilding(BuildingCore.Type type)
    {
        GameObject newBuilding = Instantiate(building, new Vector3((transform.position.x + 25) + 50 * ownedBuildings.Count, 1, (transform.position.z + 25) + 50 * ownedBuildings.Count), new Quaternion(0, 0, 0, 90), transform);
        BuildingCore newBuildingCore = newBuilding.GetComponent<BuildingCore>();
        newBuildingCore.masterRegion = regionHandle;
        newBuildingCore.type = type;
        GetBuildingsInRegion();
    }
}