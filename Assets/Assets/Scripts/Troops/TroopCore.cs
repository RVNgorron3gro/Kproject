using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopCore : MonoBehaviour
{
    UI_Map menu;
    TroopHUD tHud;

    [Header("Identification")]
    public BuildingCore masterBuilding;
    public string currentLocation;
    public RegionCore currentRegion;

    [Header("Instruction")]
    public string testDestination;
    public List<int> pathInstructionSet;
    public int previousInstruction;
    //public List<int> hudInstructionSet;

    [Header("Builders")]
    public bool beginConstruction;

    public Type type;
    public enum Type
    {
        None /*Barracks*/, Ambush, Assault, Defender /*Workshop*/, BarracksBuilder, GranaryBuilder, WorkshopBuilder, SiegeCatapult, SiegeRam
    }

    public Transform regionT;

    [Header("Troop Status")]
    public float memberMaxHealth;
    public float[] memberHealth = new float[5];

    void Start()
    {
        menu = GameObject.Find("Menu").GetComponent<UI_Map>();
        tHud = transform.GetChild(0).GetComponent<TroopHUD>();
        regionT = GameObject.Find("Regions").transform;

        for (int count = 0; count < 5; count++)
        {
            memberHealth[count] = memberMaxHealth;
        }
    }

    public void Initialise()
    {
        currentLocation = masterBuilding.masterRegion;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            tHud.RefreshHUD(memberHealth, memberMaxHealth);
        }
    }

    public void Traverse(string destination)
    {
        //For now return a list with all the details of the route
        //hudInstructionSet = CalculateRoute(destination);

        //Get Unit Path
        pathInstructionSet = CalculateRoute(currentLocation, destination);
        pathInstructionSet.RemoveAt(0);

        //Get Default Route for Building
        masterBuilding.defaultRoute = CalculateRoute(masterBuilding.masterRegion, destination);

        //Update the hud
        menu.Centre_RefreshTroopPaths();
    }

    List<int> CalculateRoute(string from, string to)
    {
        int start = Helper.ConvertRegionNameToID(from);
        int destination = Helper.ConvertRegionNameToID(to);
        List<int> route = new List<int>();

        int currentID = start;
        route.Add(currentID);
        while (true)
        {
            if (start == destination)
            {
                return route;
            }
            else if (start > destination)
            {
                if (start == 3 || start == 6 || start == 9)
                {
                    start -= 3;
                }
                else
                {
                    if ((start - 3) >= destination)
                    {
                        start -= 3;
                    }
                    else
                    {
                        start--;
                    }
                }
            }
            else if (start < destination)
            {
                if (start == 2 || start == 5 || start == 8)
                {
                    start += 3;
                }
                else
                {
                    if ((start + 3) <= destination)
                    {
                        start += 3;
                    }
                    else
                    {
                        start++;
                    }
                }
            }
            route.Add(start);
        }
    }

    public void TravelToNextPoint()
    {
        if (pathInstructionSet.Count != 0)
        {
            while (true)
            {
                if (Helper.ConvertRegionNameToID(currentLocation) != pathInstructionSet[0])
                {
                    previousInstruction = pathInstructionSet[0];
                    transform.position = regionT.GetChild(pathInstructionSet[0]).position + new Vector3(100, 0, 100);
                    currentLocation = regionT.GetChild(pathInstructionSet[0]).name;
                    pathInstructionSet.Remove(pathInstructionSet[0]);
                    //hudInstructionSet.Remove(hudInstructionSet[0]);

                    if (pathInstructionSet.Count == 0)
                    {
                        //BUILDERS
                        if (Helper.ConvertUnitTypeToInt(type) >= 3 && Helper.ConvertUnitTypeToInt(type) <= 5 && !beginConstruction && previousInstruction != 0)
                        {
                            RegionCore targetRegion = regionT.GetChild(Helper.ConvertRegionNameToID(currentLocation)).GetComponent<RegionCore>();
                            if (targetRegion.owner == menu.playerCore.playerID && targetRegion.ownedBuildings.Count < 2)
                            {
                                beginConstruction = true;
                                Debug.Log(this + " Builders have begun construction!");
                            }
                        }
                    }
                    break;
                }
                else
                {
                    previousInstruction = pathInstructionSet[0];
                    pathInstructionSet.Remove(pathInstructionSet[0]);
                    //hudInstructionSet.Remove(hudInstructionSet[0]);
                    Debug.LogWarning("Removed an instruction!");
                }
            }
        }
    }

    public void ConstructionHandler()
    {
        if (beginConstruction)
        {
            //Create The Building
            BuildingCore.Type tester = BuildingCore.Type.Barracks;
            switch (type)
            {
                case Type.BarracksBuilder:
                    tester = BuildingCore.Type.Barracks;
                    break;
                case Type.GranaryBuilder:
                    tester = BuildingCore.Type.Granary;
                    break;
                case Type.WorkshopBuilder:
                    tester = BuildingCore.Type.Workshop;
                    break;
            }
            regionT.GetChild(Helper.ConvertRegionNameToID(currentLocation)).GetComponent<RegionCore>().ConstructBuilding(tester);
            Debug.Log(this + " Builders have finished construction!");
            KillUnit();
        }
    }

    public void KillUnit()
    {
        if (Helper.IsBarracksUnit(Helper.ConvertUnitTypeToInt(type)))
        {
            masterBuilding.barracks.ownedUnit.reference = null;
        }
        else
        {
            masterBuilding.workshop.ownedUnit.reference = null;
        }
        Destroy(gameObject);
    }
}