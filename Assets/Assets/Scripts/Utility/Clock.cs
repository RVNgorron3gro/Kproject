using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Clock : NetworkBehaviour
{
    [HideInInspector] public static Clock i;
    public static float tickLength = 0.25f;

    [Header("Values")]
    [SyncVar]
    public int turn;
    [SyncVar]
    public int totalTurn;
    public int nextTurn = 15;
    public float time = 0;
    public float timeRate = 1;
    [SyncVar]
    public float timePCT;

    [Header("Lighting")]
    public LightingData data;
    public Light sun;

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
        sun = GameObject.Find("Sun").GetComponent<Light>();

        if (isServer)
        {
            StartCoroutine(Ticker());
            RpcUpdateClock();
        }
    }

    [ServerCallback]
    void Update()
    {
        PassTime(timeRate);
        RpcUpdateLighting();
    }

    [ClientRpc]
    void RpcUpdateLighting()
    {
        sun.intensity = Mathf.Lerp(sun.intensity, data.sunIntensity[turn], 0.1f);
        sun.color = Color.Lerp(sun.color, data.sunColor[turn], 0.1f);
        RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, data.ambientIntensity[turn], 0.1f);
    }

    [Server]
    void PassTurn()
    {
        //weather changes for every region
        //units move, engage in combat, etc
        //buildings under construction are built
        //units that are queued in buildings are spawned
        //neutrals are spawned, the number of neutrals spawned depends on the weather: sunny = 1; cloudy = 1 to 2; foggy = 2 to 3;

        /*

        if (GetComponent<LandStatus>().currentneutrals < GetComponent<LandStatus>().maxneutrals)
        {
            GetComponent<LandStatus>().currentneutrals += GetComponent<LandStatus>().growth;
            if (GetComponent<LandStatus>().currentneutrals > GetComponent<LandStatus>().maxneutrals)
            {
                GetComponent<LandStatus>().currentneutrals = GetComponent<LandStatus>().maxneutrals;
            }
        }
        //oh god, so many things happen when a turn is changed, please don't kill me this game will be amazing
        */

        //Update All Regions
        GameObject[] regions = GameStatus.i.GetAllRegions();
        for (int count = 0; count < regions.Length; count++)
        {
            regions[count].GetComponent<RegionCore>().PassTurn();
        }

        if (turn >= 7)
        {
            turn = 0;
        }
        else
        {
            turn++;
        }
        totalTurn++;

        RpcUpdateClock();
        UI_Chat.i.AddSystemMessageToChat("Turn " + totalTurn);

        //Now Find every unit, and traverse them if needed
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");
        for (int count = 0; count < troops.Length; count++)
        {
            TroopCore target = troops[count].GetComponent<TroopCore>();
            target.TravelToNextPoint();
            target.ConstructionHandler();
        }

        /*
        UI_Map map = UI_Map.i;
        if (map.active)
        {
            map.Centre_RefreshTroopPaths();
            map.Centre_RefreshTroopPins();
        }
        */

        time = 0;
    }

    [Server]
    public void TimeRateModifier(bool battle)
    {
        if (battle)
        {
            timeRate = 0.25f;
        }
        else
        {
            timeRate = 1;
        }
    }

    [Server]
    void PassTime(float timeRate)
    {
        time += timeRate * Time.deltaTime;

        //If time equals next turn we go to the next turn
        if (time >= nextTurn)
        {
            PassTurn();
        }

        timePCT = time / nextTurn;
    }

    [ClientRpc]
    void RpcUpdateClock()
    {
        UI_HUD.i.UpdateClock(totalTurn);
    }

    [Server]
    IEnumerator Ticker()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(tickLength);
            RpcTick();
        }
    }

    [Server]
    void RpcTick()
    {
        for(int count = 0; count < GameStatus.i.unitCores.Count; count++)
        {
            GameStatus.i.unitCores[count].Tick();
            GameStatus.i.unitCores[count].GetComponent<HERO_StateController>().Tick();
            if (GameStatus.i.unitCores[count].isPlayerReviving)
            {
                GameStatus.i.unitCores[count].GetComponent<PlayerCore>().Tick();
            }
            UI_HUD.i.RequestStateUpdate();
        }
    }
}