using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class MenuController : MonoBehaviour
{
    /*
    [Header("Status")]
    public bool active;
    public CanvasGroup canvasAlpha;

    [Header("Target Scripts")]
    public GameObject player;
    public PlayerCore playerCore;

    [System.Serializable]
    public struct BuildingMode
    {
        //Left

        //Centre
        public Transform mapMaster;
        public List<Image> map;
        public List<GameObject> region;
        public GameObject regionCaptured;

        //Right
        public GameObject listObject, emptyObject, counterObject;
        public Transform list;
        public Sprite spriteBarracks, spriteGranary, spriteWorkshop;
        public int selection, oldType;
        public BuildingCore selected;

        public GameObject assault, ambush, defender;
        public GameObject queuedUnits, trainedUnits;
        public int cycleSelection;
        public Transform submenu;
        public TextMeshProUGUI selectedText;
    }
    public BuildingMode building;

    public GameObject TEST1, TEST2;

    [System.Serializable]
    public struct InventoryMode
    {

    }
    public InventoryMode inventory;

    void Start()
    {
        //Main
        canvasAlpha = GetComponent<CanvasGroup>();

        //Scripts
        player = GameObject.Find("Player");
        playerCore = player.GetComponent<PlayerCore>();

        //Get UI Components - Building Mode
        building.mapMaster = GameObject.Find("Menu/Centre/Map").transform;
        for (int count = 0; count < 12; count++)
        {
            building.map.Add(building.mapMaster.GetChild(count).GetComponent<Image>());
        }
        building.list = GameObject.Find("Menu/Right/Buildings").transform;

        building.submenu = GameObject.Find("Menu/Right/Submenu/Back").transform;
        building.selectedText = GameObject.Find("Menu/Right/Submenu/Back/Selected Text").GetComponent<TextMeshProUGUI>();

        //Initialise
        building.oldType = -1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleMenu();
        }

        //Request SubMenu information
        if (active)
        {
            if (building.selected)
            {
                switch (building.selected.type)
                {
                    case BuildingCore.Type.Barracks:
                        UpdateBarrackSlots(building.selected.GetComponent<BuildingCore.Barracks>().RequestGenerationInformation());
                        break;
                }
            }
        }
    }

    public void ToggleMenu()
    {
        active = !active;

        if (active)
        {
            canvasAlpha.alpha = 1;

            //Lighten all map regions
            for (int count = 0; count < 12; count++)
            {
                building.map[count].color = new Color(1, 1, 1);
            }
            ConstructMenu();
        }
        else
        {
            building.selected = null;
            building.selection = 0;
            building.oldType = -1;
            canvasAlpha.alpha = 0;
        }
    }

    public void ConstructMenu()
    {
        CreateBuildingsList();
        UpdateMap();
        ClearSubMenu();
    }

    void CreateBuildingsList()
    {
        //Clear old List
        for (int count = building.list.childCount - 1; count > 0; count--)
        {
            Destroy(building.list.GetChild(count).gameObject);
        }

        //Create
        PlayerCore.BuildingList buildingList = playerCore.GetOwnedBuildings();

        //Barracks
        if (buildingList.Barracks.Count != 0)
        {
            GameObject newBlock = Instantiate(building.listObject, building.list.transform);

            //Set up Block
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Barracks";
            newBlock.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = building.spriteBarracks;
            newBlock.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => SelectBuiding(0));

            //Set up Counter
            for (int count = 0; count < buildingList.Barracks.Count; count++)
            {
                GameObject newCounter = Instantiate(building.counterObject, newBlock.transform.GetChild(0).GetChild(2));
                newCounter.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingList.Barracks[count].GetComponent<BuildingCore>().HealthPCT();
            }
        }
        else
        {
            Instantiate(building.emptyObject, building.list.transform);
        }

        //Granary
        if (buildingList.Granary.Count != 0)
        {
            GameObject newBlock = Instantiate(building.listObject, building.list.transform);

            //Set up Block
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Granary";
            newBlock.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = building.spriteGranary;
            newBlock.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => SelectBuiding(1));

            //Set up Counter
            for (int count = 0; count < buildingList.Granary.Count; count++)
            {
                GameObject newCounter = Instantiate(building.counterObject, newBlock.transform.GetChild(0).GetChild(2));
                newCounter.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingList.Granary[count].GetComponent<BuildingCore>().HealthPCT();
            }
        }
        else
        {
            Instantiate(building.emptyObject, building.list.transform);
        }

        //Workshop
        if (buildingList.Workshop.Count != 0)
        {
            GameObject newBlock = Instantiate(building.listObject, building.list.transform);

            //Set up Block
            newBlock.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Workshop";
            newBlock.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = building.spriteWorkshop;
            newBlock.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => SelectBuiding(2));

            //Set up Counter
            for (int count = 0; count < buildingList.Workshop.Count; count++)
            {
                GameObject newCounter = Instantiate(building.counterObject, newBlock.transform.GetChild(0).GetChild(2));
                newCounter.transform.GetChild(0).GetComponent<Image>().fillAmount = buildingList.Workshop[count].GetComponent<BuildingCore>().HealthPCT();
            }
        }
        else
        {
            Instantiate(building.emptyObject, building.list.transform);
        }
    }

    public void SelectBuiding(int type)
    {
        PlayerCore.BuildingList buildingList = playerCore.GetOwnedBuildings();

        if (building.oldType != type)
        {
            building.oldType = type;
            building.selection = 0;
        }
        else
        {
            switch (type)
            {
                case 0:
                    building.selection = (int)Mathf.Repeat(building.selection + 1, buildingList.Barracks.Count);
                    break;
                case 1:
                    building.selection = (int)Mathf.Repeat(building.selection + 1, buildingList.Granary.Count);
                    break;
                case 2:
                    building.selection = (int)Mathf.Repeat(building.selection + 1, buildingList.Workshop.Count);
                    break;
            }
        }

        switch (type)
        {
            case 0:
                building.selected = buildingList.Barracks[building.selection];
                break;
            case 1:
                building.selected = buildingList.Granary[building.selection];
                break;
            case 2:
                building.selected = buildingList.Workshop[building.selection];
                break;
        }

        //Clear Selection
        for (int count = 1; count < building.list.transform.childCount; count++)
        {
            if (building.list.transform.GetChild(count).childCount != 0)
            {
                for (int counter = 0; counter < building.list.transform.GetChild(count).GetChild(0).GetChild(2).childCount; counter++)
                {
                    building.list.transform.GetChild(count).GetChild(0).GetChild(2).GetChild(counter).GetComponent<Outline>().effectColor = Color.clear;
                }
            }
        }

        //Show which is selected
        building.list.transform.GetChild(type + 1).GetChild(0).GetChild(2).GetChild(building.selection).GetComponent<Outline>().effectColor = Color.red;

        //Darken all map regions
        for (int count = 0; count < 12; count++)
        {
            building.map[count].color = new Color(0.25f, 0.25f, 0.25f);
        }

        //Show which region is selected
        switch (type)
        {
            case 0:
                GameObject.Find("Map/" + buildingList.Barracks[building.selection].masterRegion).GetComponent<Image>().color = Color.white;
                break;
            case 1:
                GameObject.Find("Map/" + buildingList.Granary[building.selection].masterRegion).GetComponent<Image>().color = Color.white;
                break;
            case 2:
                GameObject.Find("Map/" + buildingList.Workshop[building.selection].masterRegion).GetComponent<Image>().color = Color.white;
                break;
        }

        //Now create the Submenu
        CreateSubMenu(type);
    }

    void ClearSubMenu()
    {
        Debug.Log("Destroyed");

        building.selectedText.text = "Select a Building";

        for (int count = building.submenu.childCount - 1; count > 0; count--)
        {
            DestroyImmediate(building.submenu.GetChild(count).gameObject);
        }
    }

    public void CreateSubMenu(int type)
    {
        //Clear old Submenu
        ClearSubMenu();

        if (type == 0)
        {
            //Update Text
            building.selectedText.text = "Barracks (" + building.selected.masterRegion + ")";

            //Add Queued Units
            GameObject newQueuedUnits = Instantiate(building.queuedUnits, building.submenu.transform);

            //Add Buttons 
            TEST1 = newQueuedUnits.transform.GetChild(0).GetChild(2).gameObject;
            TEST2 = newQueuedUnits.transform.GetChild(1).GetChild(2).gameObject;
            newQueuedUnits.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => CycleUnitGeneration(-1));
            newQueuedUnits.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(() => CycleUnitGeneration(0));

            for (int slot = 0; slot < 2; slot++)
            {
                if (!building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].reference)
                {
                    switch (building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].unit)
                    {
                        case UnitCore.Type.Assault:
                            Instantiate(building.assault, building.submenu.GetChild(1).GetChild(slot).GetChild(0));
                            break;
                        case UnitCore.Type.Ambush:
                            Instantiate(building.ambush, building.submenu.GetChild(1).GetChild(slot).GetChild(0));
                            break;
                        case UnitCore.Type.Defender:
                            Instantiate(building.defender, building.submenu.GetChild(1).GetChild(slot).GetChild(0));
                            break;
                    }
                }
            }

            //Add Trained Units
            Instantiate(building.trainedUnits, building.submenu.transform);

            for (int slot = 0; slot < 2; slot++)
            {
                if (building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].reference)
                {
                    GameObject newUnit = new GameObject();
                    switch (building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].unit)
                    {
                        case UnitCore.Type.Assault:
                            newUnit = Instantiate(building.assault, building.submenu.GetChild(2).GetChild(slot).GetChild(0));
                            break;
                        case UnitCore.Type.Ambush:
                            newUnit = Instantiate(building.ambush, building.submenu.GetChild(2).GetChild(slot).GetChild(0));
                            break;
                        case UnitCore.Type.Defender:
                            newUnit = Instantiate(building.defender, building.submenu.GetChild(2).GetChild(slot).GetChild(0));
                            break;
                    }

                    //Set up the draggable script attached
                    newUnit.GetComponent<Draggable>().unit = building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].reference;
                }
            }

        }
        else if (type == 2)
        {
            building.selectedText.text = "Workshop (" + building.selected.masterRegion + ")";
        }
    }

    void CycleUnitGeneration(int slot)
    {
        UnitCore.Type toQueue = UnitCore.Type.None;

        Debug.Log(slot);
        switch (building.selected.GetComponent<BuildingCore.Barracks>().ownedUnit[slot].unit)
        {
            case UnitCore.Type.None:
                building.cycleSelection = -1;
                break;
            case UnitCore.Type.Assault:
                building.cycleSelection = 0;
                break;
            case UnitCore.Type.Ambush:
                building.cycleSelection = 1;
                break;
            case UnitCore.Type.Defender:
                building.cycleSelection = 2;
                break;
        }

        //Add 1
        building.cycleSelection = (int)Mathf.Repeat(building.cycleSelection + 1, 2);

        switch (building.cycleSelection)
        {
            case 0:
                toQueue = UnitCore.Type.Assault;
                break;
            case 1:
                toQueue = UnitCore.Type.Ambush;
                break;
            case 2:
                toQueue = UnitCore.Type.Defender;
                break;
        }
        building.selected.GetComponent<BuildingCore.Barracks>().QueueNewUnit(toQueue, slot);

        CreateSubMenu(0);
    }

    void UpdateBarrackSlots(float[] times)
    {
        for (int slot = 0; slot < 2; slot++)
        {
            //Queued Units
            if (times[slot] != 0)
            {
                building.submenu.GetChild(1).GetChild(slot).GetChild(1).GetComponent<Image>().fillAmount = times[slot] / 15;
                building.submenu.GetChild(1).GetChild(slot).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = times[slot].ToString("F1");
            }
            else
            {
                building.submenu.GetChild(1).GetChild(slot).GetChild(1).GetComponent<Image>().fillAmount = 0;
                building.submenu.GetChild(1).GetChild(slot).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    void UpdateMap()
    {
        for (int count = 0; count < 12; count++)
        {
            for (int child = 0; child < building.mapMaster.GetChild(count).childCount; child++)
            {
                Destroy(building.mapMaster.GetChild(count).GetChild(child).gameObject);
            }

            for (int region = 0; region < playerCore.GetOwnedRegions().Count; region++)
            {
                if (playerCore.GetOwnedRegions()[region].regionHandle == building.mapMaster.GetChild(count).name)
                {
                    Instantiate(building.regionCaptured, building.mapMaster.GetChild(count));
                }
            }
        }
    }
    */
}