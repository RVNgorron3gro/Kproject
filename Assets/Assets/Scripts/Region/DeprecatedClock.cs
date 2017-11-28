using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DeprecatedClock
{
    /* DEPRECATED, USE CLOCK METHODS INSTEAD
    [Header("Values")]
    public static int turn;
    public static int totalTurn;
    public static int nextTurn = 15;
    public static float time = 0;
    public static float timeRate = 10;
    public static float timePCT;

    static void PassTurn()
    {
        //weather changes for every region
        //units move, engage in combat, etc
        //buildings under construction are built
        //units that are queued in buildings are spawned
        //neutrals are spawned, the number of neutrals spawned depends on the weather: sunny = 1; cloudy = 1 to 2; foggy = 2 to 3;

        if (GetComponent<LandStatus>().currentneutrals < GetComponent<LandStatus>().maxneutrals)
        {
            GetComponent<LandStatus>().currentneutrals += GetComponent<LandStatus>().growth;
            if (GetComponent<LandStatus>().currentneutrals > GetComponent<LandStatus>().maxneutrals)
            {
                GetComponent<LandStatus>().currentneutrals = GetComponent<LandStatus>().maxneutrals;
            }
        }
        //oh god, so many things happen when a turn is changed, please don't kill me this game will be amazing

        //Update All Regions
        List<RegionCore> regions = Helper.GetAllRegionCores();
        for(int count = 0; count < regions.Count; count++)
        {
            regions[count].PassTurn();
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
        Debug.Log("=<TURN " + totalTurn + ">=");

        //Now Find every unit, and traverse them if needed
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");
        for(int count = 0; count < troops.Length; count++)
        {
            troops[count].GetComponent<UnitCore>().TravelToNextPoint();
        }

        HERO_MenuController menu = GameObject.Find("Menu").GetComponent<HERO_MenuController>();
        if (menu.active)
        {
            menu.Centre_RefreshTroopPaths();
            menu.Centre_RefreshTroopPins();
        }

        time = 0;
    }

    static void TimeRateModifierMthd()
    {
        //if both main characters are in the same region, time will count at 0.25
        //if (encounter){ timeRate = 0.25; } else { timerate = 1; }
    }

    public static void PassTime(float timeRate)
    {
        time += timeRate * Time.deltaTime;

        //If time equals next turn we go to the next turn
        if (time >= nextTurn)
        {
            PassTurn();
        }

        timePCT = time / nextTurn;
    }
*/
}