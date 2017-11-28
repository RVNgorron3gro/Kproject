using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static int ConvertRegionNameToID(string regionName)
    {
        switch (regionName)
        {
            case "Beach":
                return 0;
            case "Fishing Village":
                return 1;
            case "Swamp":
                return 2;
            case "Badlands":
                return 3;
            case "Ruins":
                return 4;
            case "Player 1 Start":
                return 5;
            case "Player 2 Start":
                return 6;
            case "Farmlands":
                return 7;
            case "Forest":
                return 8;
            case "Lake":
                return 9;
            case "Frozen Village":
                return 10;
            case "Mountain":
                return 11;
            default:
                return -1;
        }
    }

    public static List<BuildingCore> GetAllPlayerAOwnedBuildings()
    {
        List<BuildingCore> list = new List<BuildingCore>();
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        for (int count = 0; count < buildings.Length; count++)
        {
            if (buildings[count].GetComponent<BuildingCore>().owner == BuildingCore.Owner.Player1)
            {
                list.Add(buildings[count].GetComponent<BuildingCore>());
            }
        }
        return list;
    }

    public static List<BuildingCore> GetAllPlayerBOwnedBuildings()
    {
        List<BuildingCore> list = new List<BuildingCore>();
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        for (int count = 0; count < buildings.Length; count++)
        {
            if (buildings[count].GetComponent<BuildingCore>().owner == BuildingCore.Owner.Player2)
            {
                list.Add(buildings[count].GetComponent<BuildingCore>());
            }
        }
        return list;
    }

    public static int ConvertUnitTypeToInt(TroopCore.Type type)
    {
        switch (type)
        {
            case TroopCore.Type.Ambush:
                return 0;
            case TroopCore.Type.Assault:
                return 1;
            case TroopCore.Type.Defender:
                return 2;
            case TroopCore.Type.BarracksBuilder:
                return 3;
            case TroopCore.Type.GranaryBuilder:
                return 4;
            case TroopCore.Type.WorkshopBuilder:
                return 5;
            case TroopCore.Type.SiegeCatapult:
                return 6;
            case TroopCore.Type.SiegeRam:
                return 7;
            case TroopCore.Type.None:
                return -1;
            default:
                return -1;
        }
    }

    public static TroopCore.Type ConvertUnitIntToType(int type)
    {
        switch (type)
        {
            case 0:
                return TroopCore.Type.Ambush;
            case 1:
                return TroopCore.Type.Assault;
            case 2:
                return TroopCore.Type.Defender;
            case 3:
                return TroopCore.Type.BarracksBuilder;
            case 4:
                return TroopCore.Type.GranaryBuilder;
            case 5:
                return TroopCore.Type.WorkshopBuilder;
            case 6:
                return TroopCore.Type.SiegeCatapult;
            case 7:
                return TroopCore.Type.SiegeRam;
            default:
                return TroopCore.Type.None;
        }
    }

    public static bool IsBarracksUnit(int type)
    {
        if (type <= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}