using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameStatus : NetworkBehaviour
{
    public static GameStatus i;
    public List<Player> players = new List<Player>();
    public List<UnitCore> unitCores = new List<UnitCore>();

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

    [Server]
    public void ReportConnected(NetworkInstanceId newConnection)
    {
        players.Add(NetworkServer.FindLocalObject(newConnection).GetComponent<Player>());
        players[players.Count - 1].CmdConnected(players.Count - 1);
    }

    [Server]
    public void ManageUnitCores(UnitCore target, bool register)
    {
        if (register)
        {
            unitCores.Add(target);
        }
        else
        {
            unitCores.Remove(target);
        }
    }

    public GameObject[] GetAllRegions()
    {
        return GameObject.FindGameObjectsWithTag("Region");
    }

    public Color ClassColor(Defs.SlotType target)
    {
        switch (target)
        {
            case Defs.SlotType.Red:
                return Color.red;
            case Defs.SlotType.Blue:
                return Color.blue;
            case Defs.SlotType.Green:
                return Color.green;
            default:
                return Color.black;
        }
    }

    public Color ClassColor(int target)
    {
        return ClassColor((Defs.SlotType)target);
    }
}