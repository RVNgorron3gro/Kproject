using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class BuildingCore : MonoBehaviour
{
    public Barracks barracks; public Granary granary; public Workshop workshop;
    public GameObject ambush, assault, defender;
    public GameObject barracksBuilder, granaryBuilder, workshopBuilder, siegeCatapult, siegeRam;

    [Header("Identification")]
    public string masterRegion;

    public Type type;
    public enum Type
    {
        Barracks, Granary, Workshop
    }

    public Owner owner;
    public enum Owner
    {
        Player1, Player2
    }

    public List<int> defaultRoute;

    public float healthMax, health;

    public float HealthPCT()
    {
        return Mathf.Clamp(health / healthMax, 0, 1);
    }

    void Start()
    {
        //Initialise
        health = healthMax;

        //Add appropriate building script
        switch (type)
        {
            case Type.Barracks:
                barracks = gameObject.AddComponent<Barracks>();
                break;
            case Type.Granary:
                granary = gameObject.AddComponent<Granary>();
                break;
            case Type.Workshop:
                workshop = gameObject.AddComponent<Workshop>();
                break;
        }
    }

    [DisallowMultipleComponent]
    public class Barracks : MonoBehaviour
    {
        UI_Map menu;

        [Header("Owned Units")]
        public OwnedUnit ownedUnit;
        [System.Serializable]
        public struct OwnedUnit
        {
            public TroopCore.Type unit;
            public TroopCore.Type queued;
            public float generationTime;
            public TroopCore reference;
        }

        void Start()
        {
            menu = GameObject.Find("HUD/Menu").GetComponent<UI_Map>();
            ownedUnit = new OwnedUnit();
        }

        void Update()
        {
            if (ownedUnit.queued != TroopCore.Type.None)
            {
                if (!ownedUnit.reference)
                {
                    if (ownedUnit.generationTime == 0)
                    {
                        InstantiateUnit();
                    }
                    else
                    {
                        ownedUnit.generationTime = Mathf.Clamp(ownedUnit.generationTime - Time.deltaTime, 0, 15);
                    }
                }
            }
        }

        public void QueueNewUnit(TroopCore.Type type)
        {
            if (ownedUnit.reference)
            {
                ownedUnit.queued = type;
            }
            else
            {
                if (ownedUnit.generationTime == 0)
                {
                    ownedUnit.generationTime = 15;
                }
                ownedUnit.queued = type;
            }
        }

        void InstantiateUnit()
        {
            GameObject newUnit;
            ownedUnit.unit = ownedUnit.queued;

            if (ownedUnit.reference)
            {
                Debug.Log("Hit");
                Destroy(ownedUnit.reference.gameObject);
                ownedUnit.reference = null;
            }

            switch (ownedUnit.queued)
            {
                case TroopCore.Type.Assault:
                    newUnit = Instantiate(GetComponent<BuildingCore>().assault);
                    break;
                case TroopCore.Type.Ambush:
                    newUnit = Instantiate(GetComponent<BuildingCore>().ambush);
                    break;
                case TroopCore.Type.Defender:
                    newUnit = Instantiate(GetComponent<BuildingCore>().defender);
                    break;
                case TroopCore.Type.None:
                    newUnit = null;
                    Debug.LogError("NULL TYPE UNIT QUEUED");
                    break;
                default:
                    newUnit = null;
                    Debug.LogError("NON WORKSHOP UNIT QUEUED IN WORKSHOP BUILDING");
                    break;
            }
            ownedUnit.reference = newUnit.GetComponent<TroopCore>();
            newUnit.GetComponent<TroopCore>().masterBuilding = GetComponent<BuildingCore>();
            newUnit.GetComponent<TroopCore>().Initialise();
            ownedUnit.reference.type = ownedUnit.unit;

            //Update the menu if needed
            menu.Right_PingBarracksSlot(GetComponent<BuildingCore>());

            ownedUnit.generationTime = 16;
        }

        public float RequestGenerationInformation()
        {
            return ownedUnit.generationTime;
        }

        /*
        public int ConvertTypeToInt(int slot)
        {
            switch (ownedUnit.queued)
            {
                case UnitCore.Type.Ambush:
                    return 0;
                case UnitCore.Type.Assault:
                    return 1;
                case UnitCore.Type.Defender:
                    return 2;
                case UnitCore.Type.None:
                    return -1;
                default:
                    return -1;
            }
        }

        public UnitCore.Type ConvertIntToType(int type)
        {
            switch (type)
            {
                case 0:
                    return UnitCore.Type.Ambush;
                case 1:
                    return UnitCore.Type.Assault;
                case 2:
                    return UnitCore.Type.Defender;
                default:
                    return UnitCore.Type.None;
            }
        }
        */
    }

    [DisallowMultipleComponent]
    public class Granary : MonoBehaviour
    {

    }

    [DisallowMultipleComponent]
    public class Workshop : MonoBehaviour
    {
        UI_Map menu;

        [Header("Owned Units")]
        public OwnedUnit ownedUnit;
        [System.Serializable]
        public struct OwnedUnit
        {
            public TroopCore.Type unit;
            public TroopCore.Type queued;
            public float generationTime;
            public TroopCore reference;
        }

        void Start()
        {
            menu = GameObject.Find("HUD/Menu").GetComponent<UI_Map>();
            ownedUnit = new OwnedUnit();
        }

        void Update()
        {
            if (ownedUnit.queued != TroopCore.Type.None)
            {
                if (!ownedUnit.reference)
                {
                    if (ownedUnit.generationTime == 0)
                    {
                        InstantiateUnit();
                    }
                    else
                    {
                        ownedUnit.generationTime = Mathf.Clamp(ownedUnit.generationTime - Time.deltaTime, 0, 15);
                    }
                }
            }
        }

        public void QueueNewUnit(TroopCore.Type type)
        {
            if (ownedUnit.reference)
            {
                ownedUnit.queued = type;
            }
            else
            {
                if (ownedUnit.generationTime == 0)
                {
                    ownedUnit.generationTime = 15;
                }
                ownedUnit.queued = type;
            }
        }

        void InstantiateUnit()
        {
            GameObject newUnit;
            ownedUnit.unit = ownedUnit.queued;

            if (ownedUnit.reference)
            {
                Destroy(ownedUnit.reference.gameObject);
                ownedUnit.reference = null;
            }

            switch (ownedUnit.queued)
            {
                case TroopCore.Type.BarracksBuilder:
                    newUnit = Instantiate(GetComponent<BuildingCore>().barracksBuilder);
                    break;
                case TroopCore.Type.GranaryBuilder:
                    newUnit = Instantiate(GetComponent<BuildingCore>().granaryBuilder);
                    break;
                case TroopCore.Type.WorkshopBuilder:
                    newUnit = Instantiate(GetComponent<BuildingCore>().workshopBuilder);
                    break;
                case TroopCore.Type.SiegeCatapult:
                    newUnit = Instantiate(GetComponent<BuildingCore>().siegeCatapult);
                    break;
                case TroopCore.Type.SiegeRam:
                    newUnit = Instantiate(GetComponent<BuildingCore>().siegeRam);
                    break;
                case TroopCore.Type.None:
                    newUnit = null;
                    Debug.LogError("NULL TYPE UNIT QUEUED");
                    break;
                default:
                    newUnit = null;
                    Debug.LogError("NON WORKSHOP UNIT QUEUED IN WORKSHOP BUILDING");
                    break;
            }
            ownedUnit.reference = newUnit.GetComponent<TroopCore>();
            newUnit.GetComponent<TroopCore>().masterBuilding = GetComponent<BuildingCore>();
            newUnit.GetComponent<TroopCore>().Initialise();
            ownedUnit.reference.type = ownedUnit.unit;

            //Update the menu if needed
            menu.Right_PingWorkshopSlot(GetComponent<BuildingCore>());

            ownedUnit.generationTime = 16;
        }

        public float RequestGenerationInformation()
        {
            return ownedUnit.generationTime;
        }
    }
}