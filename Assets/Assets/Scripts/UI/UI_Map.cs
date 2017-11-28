using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    [HideInInspector] public static UI_Map i;

    [Header("References")]
    public PlayerCore playerCore;

    [Header("Status")]
    public bool active = false;
    public CanvasGroup canvas;

    [Header("Centre")]
    public Transform centre;
    public Transform centre_MapT;
    public Transform centre_NavT;

    [Header("Right")]
    public Transform right;
    public Transform right_BuildingsT;
    public int right_BuildingCycle = -1; public int right_BuildingType = -1; public BuildingCore right_BuildingSelected;
    public Transform right_SubmenuT;
    public TextMeshProUGUI right_SubmenuSelectedText;
    public Image right_SubmenuBuildingHealthBar;
    public TextMeshProUGUI right_SubmenuBuildingHealthText;

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

    public void Setup(PlayerCore targetPlayer)
    {
        //References
        playerCore = targetPlayer;

        //Status
        canvas = GetComponent<CanvasGroup>();

        //Centre Components
        centre = GameObject.Find("Map/Centre").transform;
        centre_MapT = centre.GetChild(0).transform;
        centre_NavT = centre.GetChild(1).transform;

        //Right Componenets
        right = GameObject.Find("Map/Right").transform;
        right_BuildingsT = right.GetChild(0);
        right_SubmenuT = right.GetChild(1).GetChild(0);
        right_SubmenuSelectedText = right_SubmenuT.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        right_SubmenuBuildingHealthBar = right_SubmenuT.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        right_SubmenuBuildingHealthText = right_SubmenuBuildingHealthBar.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public enum HighLightMode
    {
        Lighten, Darken, Reden
    }

    public void Centre_HighlightMap(int region, HighLightMode mode)
    {
        Color to = Color.white;
        switch (mode)
        {
            case HighLightMode.Darken:
                to = new Color(0.4f, 0.4f, 0.4f);
                break;
            case HighLightMode.Lighten:
                to = Color.white;
                break;
            case HighLightMode.Reden:
                to = Color.red;
                break;
        }
        centre_MapT.GetChild(region).GetComponent<Image>().color = to;
    }

    public void Centre_InitialiseMap(HighLightMode mode)
    {
        for (int count = 0; count < 12; count++)
        {
            Centre_HighlightMap(count, mode);
        }
    }

    public enum HighlightCritera
    {
        BuildableRegions
    }

    public void Centre_HighlightMap(HighlightCritera criteria)
    {
        Transform regions = GameObject.Find("Regions").transform;

        //Darken the whole map first
        Centre_InitialiseMap(HighLightMode.Darken);
        switch (criteria)
        {
            case HighlightCritera.BuildableRegions:
                for (int count = 0; count < 12; count++)
                {
                    RegionCore targetRegion = regions.GetChild(count).GetComponent<RegionCore>();
                    if (targetRegion)
                    {
                        if (targetRegion.owner == playerCore.playerID)
                        {
                            if (targetRegion.ownedBuildings.Count < 2)
                            {
                                Centre_HighlightMap(count, HighLightMode.Lighten);
                            }
                            else
                            {
                                Centre_HighlightMap(count, HighLightMode.Reden);
                            }
                        }
                    }
                }
                break;
        }
    }

   public  void Centre_HighlightMap(string region, HighLightMode mode)
    {
        Centre_HighlightMap(Helper.ConvertRegionNameToID(region), mode);
    }

    void Centre_UpdateMapRegions()
    {
        GameObject[] regions = GameStatus.i.GetAllRegions();
        for (int count = 0; count < 12; count++)
        {
            if (regions[count].GetComponent<RegionCore>())
            {
                //Capture
                bool capped = false;
                if (regions[count].GetComponent<RegionCore>().owner == playerCore.playerID)
                {
                    capped = true;
                }
                centre_MapT.GetChild(count).GetChild(0).gameObject.SetActive(capped);


                //Buildings 
                for (int element = 0; element < 6; element++)
                {
                    centre_MapT.GetChild(count).GetChild(1).GetChild(element).gameObject.SetActive(false);
                }

                if (capped)
                {
                    for (int building = 0; building < regions[count].GetComponent<RegionCore>().ownedBuildings.Count; building++)
                    {
                        int type = -1;
                        switch (regions[count].GetComponent<RegionCore>().ownedBuildings[building].type)
                        {
                            case BuildingCore.Type.Barracks:
                                type = 0;
                                break;
                            case BuildingCore.Type.Granary:
                                type = 2;
                                break;
                            case BuildingCore.Type.Workshop:
                                type = 4;
                                break;
                        }
                        if (centre_MapT.GetChild(count).GetChild(1).GetChild(type).gameObject.activeInHierarchy)
                            type++;

                        centre_MapT.GetChild(count).GetChild(1).GetChild(type).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                //Capture
                centre_MapT.GetChild(count).GetChild(0).gameObject.SetActive(false);

                //Buildings - This is temporary
                for (int element = 0; element < 6; element++)
                {
                    centre_MapT.GetChild(count).GetChild(1).GetChild(element).gameObject.SetActive(false);
                }
            }
        }
    }

    void Centre_ClearMapPathing()
    {
        for (int region = 0; region < 12; region++)
        {
            for (int dir = 0; dir < 4; dir++)
            {
                centre_NavT.GetChild(region).GetChild(dir).GetChild(0).GetComponent<Image>().color = Color.clear;
            }
        }
    }

    public void Centre_DrawBuildingDefaultPath(BuildingCore target)
    {
        List<int> path = target.defaultRoute;
        for (int instruction = 0; instruction < path.Count - 1; instruction++)
        {
            int action = path[instruction + 1] - path[instruction];
            int dir = 0;
            switch (action)
            {
                //UP
                case -3:
                    dir = 0;
                    break;
                //DOWN
                case 3:
                    dir = 1;
                    break;
                //LEFT
                case -1:
                    dir = 2;
                    break;
                //RIGHT
                case 1:
                    dir = 3;
                    break;
            }

            TroopCore.Type tester = TroopCore.Type.None;
            if(target.type == BuildingCore.Type.Barracks)
            {
                tester = target.barracks.ownedUnit.queued;
            }
            else if(target.type == BuildingCore.Type.Workshop)
            {
                tester = target.workshop.ownedUnit.queued;
            }

            Color to = Color.white;
            switch (tester)
            {
                case TroopCore.Type.Ambush:
                    to = Color.green;
                    break;
                case TroopCore.Type.Assault:
                    to = Color.red;
                    break;
                case TroopCore.Type.Defender:
                    to = Color.blue;
                    break;
                case TroopCore.Type.BarracksBuilder:
                    to = Color.yellow;
                    break;
                case TroopCore.Type.GranaryBuilder:
                    to = Color.yellow;
                    break;
                case TroopCore.Type.WorkshopBuilder:
                    to = Color.yellow;
                    break;
                case TroopCore.Type.SiegeCatapult:
                    to = Color.gray;
                    break;
                case TroopCore.Type.SiegeRam:
                    to = Color.gray;
                    break;
            }
            centre_NavT.GetChild(path[instruction]).GetChild(dir).GetChild(0).GetComponent<Image>().color = to;
        }
    }

    public void Centre_RefreshTroopPaths()
    {
        Centre_ClearMapPathing();
        if (right_BuildingSelected)
        {
            Centre_DrawBuildingDefaultPath(right_BuildingSelected);
        }
    }

    public void Centre_RefreshTroopPins()
    {
        Centre_ClearTroopPins();

        List<BuildingCore> ownedBuildings = Helper.GetAllPlayerAOwnedBuildings();
        List<TroopCore> ownedUnits = new List<TroopCore>();

        for (int building = 0; building < ownedBuildings.Count; building++)
        {
            if (ownedBuildings[building].barracks)
            {
                if (ownedBuildings[building].barracks.ownedUnit.reference)
                {
                    ownedUnits.Add(ownedBuildings[building].barracks.ownedUnit.reference);
                }
            }
            else if (ownedBuildings[building].workshop)
            {
                if (ownedBuildings[building].workshop.ownedUnit.reference)
                {
                    ownedUnits.Add(ownedBuildings[building].workshop.ownedUnit.reference);
                }
            }
        }

        GameObject[] regionObjects = GameStatus.i.GetAllRegions();
        for (int unit = 0; unit < ownedUnits.Count; unit++)
        {
            for (int region = 0; region < 12; region++)
            {
                if (ownedUnits[unit].currentLocation == regionObjects[region].name)
                {
                    int start = 0;
                    switch (ownedUnits[unit].type)
                    {
                        case TroopCore.Type.Ambush:
                            start = 0;
                            break;
                        case TroopCore.Type.Assault:
                            start = 9;
                            break;
                        case TroopCore.Type.Defender:
                            start = 18;
                            break;
                        case TroopCore.Type.BarracksBuilder:
                            start = 27;
                            break;
                        case TroopCore.Type.GranaryBuilder:
                            start = 36;
                            break;
                        case TroopCore.Type.WorkshopBuilder:
                            start = 45;
                            break;
                        case TroopCore.Type.SiegeCatapult:
                            start = 54;
                            break;
                        case TroopCore.Type.SiegeRam:
                            start = 63;
                            break;
                        case TroopCore.Type.None:
                            break;
                    }

                    while (true)
                    {
                        if (!centre_MapT.GetChild(region).GetChild(2).GetChild(start).gameObject.activeSelf)
                        {
                            centre_MapT.GetChild(region).GetChild(2).GetChild(start).gameObject.SetActive(true);
                            break;
                        }
                        else
                        {
                            start++;
                        }
                    }
                }
            }
        }
    }

    void Centre_ClearTroopPins()
    {
        for (int region = 0; region < 12; region++)
        {
            for (int count = 0; count < 72; count++)
            {
                centre_MapT.GetChild(region).GetChild(2).GetChild(count).gameObject.SetActive(false);
            }
        }
    }

    void Right_CreateBuildingList()
    {
        Right_ToggleBuildingActive(0, playerCore.GetOwnedBuildings().Barracks.Count);
        Right_ToggleBuildingActive(1, playerCore.GetOwnedBuildings().Granary.Count);
        Right_ToggleBuildingActive(2, playerCore.GetOwnedBuildings().Workshop.Count);
    }

    void Right_ToggleBuildingActive(int type, int count)
    {
        //Determine if we own this building
        bool to; if (count > 0) { to = true; } else { to = false; }

        //Now either show or hide this building
        right_BuildingsT.GetChild(type + 1).gameObject.SetActive(to);

        //If we have this building, show how many we have
        if (to == true && count != 0)
        {
            for (int structures = 0; structures < 26; structures++)
            {
                if (count > structures)
                {
                    Right_GetUIBuilding(type).GetChild(0).GetChild(2).GetChild(structures).gameObject.SetActive(true);
                }
                else
                {
                    Right_GetUIBuilding(type).GetChild(0).GetChild(2).GetChild(structures).gameObject.SetActive(false);
                }
            }
        }
    }

    public void Right_SelectBuilding(int type)
    {
        if (right_BuildingType != type)
        {
            right_BuildingCycle = -1;
        }

        right_BuildingType = type;
        right_BuildingCycle = (int)Mathf.Repeat(right_BuildingCycle + 1, Right_GetBuildingAmount(type));
        right_BuildingSelected = Right_GetOwnedBuilding(type, right_BuildingCycle);

        //Initialise the counters
        Right_InitialiseCounters();

        //Show that this building is selected on the counters
        Right_GetUIBuilding(type).GetChild(0).GetChild(2).GetChild(right_BuildingCycle).GetComponent<Outline>().effectColor = Color.red;

        //Show the contents of this building
        Right_ShowBuildingContents(right_BuildingSelected);

        //Show which region this building belongs to
        Centre_InitialiseMap(HighLightMode.Darken);
        Centre_HighlightMap(right_BuildingSelected.masterRegion, HighLightMode.Lighten);
    }

    int Right_GetBuildingAmount(int type)
    {
        switch (type)
        {
            case 0:
                return playerCore.GetOwnedBuildings().Barracks.Count;
            case 1:
                return playerCore.GetOwnedBuildings().Granary.Count;
            case 2:
                return playerCore.GetOwnedBuildings().Workshop.Count;
            default:
                return 0;
        }
    }

    Transform Right_GetUIBuilding(int type)
    {
        return right_BuildingsT.GetChild(type + 1);
    }

    void Right_InitialiseCounters()
    {
        for (int element = 0; element < 3; element++)
        {
            for (int counter = 0; counter < Right_GetUIBuilding(element).GetChild(0).GetChild(2).childCount; counter++)
            {
                Right_GetUIBuilding(element).GetChild(0).GetChild(2).GetChild(counter).GetComponent<Outline>().effectColor = Color.clear;
            }
        }
    }

    BuildingCore Right_GetOwnedBuilding(int type, int id)
    {
        switch (type)
        {
            case 0:
                return playerCore.GetOwnedBuildings().Barracks[right_BuildingCycle];
            case 1:
                return playerCore.GetOwnedBuildings().Granary[right_BuildingCycle];
            case 2:
                return playerCore.GetOwnedBuildings().Workshop[right_BuildingCycle];
            default:
                return null;
        }
    }

    void Right_ShowBuildingContents(BuildingCore target)
    {
        //Health
        right_SubmenuBuildingHealthBar.fillAmount = right_BuildingSelected.HealthPCT();
        right_SubmenuBuildingHealthText.text = right_BuildingSelected.health.ToString("F0") + "/<b>" + right_BuildingSelected.healthMax.ToString("F0") + "</b>";

        switch (target.type)
        {
            case BuildingCore.Type.Barracks:
                right_SubmenuSelectedText.text = "Barracks (" + target.masterRegion + ")";
                Right_ToggleUIBuilding(0);

                //QUEUED UNITS

                //Initialise units
                for (int unit = 0; unit < 3; unit++)
                {
                    Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(0).GetChild(unit).gameObject.SetActive(false);
                }

                GameObject barracksTargetQueued;
                switch (target.barracks.ownedUnit.queued)
                {
                    case TroopCore.Type.Ambush:
                        //targetQueuedUnit = Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        barracksTargetQueued = GameObject.Find("Barracks/Queued Units/Slot 1/Unit Parent/Ambush");
                        barracksTargetQueued.SetActive(true);
                        Right_UpdateBarracksSlot(target.barracks);
                        break;
                    case TroopCore.Type.Assault:
                        //targetQueuedUnit = Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
                        barracksTargetQueued = GameObject.Find("Barracks/Queued Units/Slot 1/Unit Parent/Assault");
                        barracksTargetQueued.SetActive(true);
                        Right_UpdateBarracksSlot(target.barracks);
                        break;
                    case TroopCore.Type.Defender:
                        //targetQueuedUnit = Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
                        barracksTargetQueued = GameObject.Find("Barracks/Queued Units/Slot 1/Unit Parent/Defender");
                        barracksTargetQueued.SetActive(true);
                        Right_UpdateBarracksSlot(target.barracks);
                        break;
                    case TroopCore.Type.None:
                        break;
                }


                //TRAINED UNITS

                //Initialise units
                for (int unit = 0; unit < 3; unit++)
                {
                    Right_GetUISubmenuBuilding(0).GetChild(1).GetChild(0).GetChild(0).GetChild(unit).gameObject.SetActive(false);
                }

                if (target.barracks.ownedUnit.reference)
                {
                    GameObject barracksTargetTrained;
                    switch (target.barracks.ownedUnit.unit)
                    {
                        case TroopCore.Type.Ambush:
                            //targetTrainedUnit = Right_GetUISubmenuBuilding(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                            barracksTargetTrained = GameObject.Find("Barracks/Trained Units/Slot 1/Unit Parent/Ambush");
                            barracksTargetTrained.SetActive(true);
                            break;
                        case TroopCore.Type.Assault:
                            //targetTrainedUnit = Right_GetUISubmenuBuilding(0).GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;
                            barracksTargetTrained = GameObject.Find("Barracks/Trained Units/Slot 1/Unit Parent/Assault");
                            barracksTargetTrained.SetActive(true);
                            break;
                        case TroopCore.Type.Defender:
                            //targetTrainedUnit = Right_GetUISubmenuBuilding(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).gameObject;
                            barracksTargetTrained = GameObject.Find("Barracks/Trained Units/Slot 1/Unit Parent/Defender");
                            barracksTargetTrained.SetActive(true);
                            break;
                        case TroopCore.Type.None:
                            barracksTargetTrained = null;
                            break;
                        default:
                            barracksTargetTrained = null;
                            break;
                    }

                    //Setup the draggable script
                    if (barracksTargetTrained != null)
                    {
                        Draggable drag = barracksTargetTrained.GetComponent<Draggable>();
                        drag.unit = target.barracks.ownedUnit.reference;
                    }
                }

                Centre_RefreshTroopPaths();
                break;
            case BuildingCore.Type.Granary:
                right_SubmenuSelectedText.text = "Granary (" + target.masterRegion + ")";
                Right_ToggleUIBuilding(1);

                Centre_RefreshTroopPaths();
                break;
            case BuildingCore.Type.Workshop:
                right_SubmenuSelectedText.text = "Workshop (" + target.masterRegion + ")";
                Right_ToggleUIBuilding(2);



                //TROOPS

                //Initialise units
                for (int unit = 0; unit < 5; unit++)
                {
                    Right_GetUISubmenuBuilding(2).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(unit).gameObject.SetActive(false);
                }

                GameObject workshopTargetQueued;

                TroopCore.Type tester;
                if (target.workshop.ownedUnit.reference)
                {
                    tester = target.workshop.ownedUnit.reference.type;
                }
                else
                {
                    tester = target.workshop.ownedUnit.queued;
                }

                switch (tester)
                {
                    case TroopCore.Type.BarracksBuilder:
                        workshopTargetQueued = GameObject.Find("Workshop/Troops/Slots/Slot 1/Background/Unit Parent/Barracks Builder");
                        workshopTargetQueued.SetActive(true);
                        Right_UpdateWorkshopSlot(target.workshop);
                        break;
                    case TroopCore.Type.GranaryBuilder:
                        workshopTargetQueued = GameObject.Find("Workshop/Troops/Slots/Slot 1/Background/Unit Parent/Granary Builder");
                        workshopTargetQueued.SetActive(true);
                        Right_UpdateWorkshopSlot(target.workshop);
                        break;
                    case TroopCore.Type.WorkshopBuilder:
                        workshopTargetQueued = GameObject.Find("Workshop/Troops/Slots/Slot 1/Background/Unit Parent/Workshop Builder");
                        workshopTargetQueued.SetActive(true);
                        Right_UpdateWorkshopSlot(target.workshop);
                        break;
                    case TroopCore.Type.SiegeCatapult:
                        workshopTargetQueued = GameObject.Find("Workshop/Troops/Slots/Slot 1/Background/Unit Parent/Siege Catapult");
                        workshopTargetQueued.SetActive(true);
                        Right_UpdateWorkshopSlot(target.workshop);
                        break;
                    case TroopCore.Type.SiegeRam:
                        workshopTargetQueued = GameObject.Find("Workshop/Troops/Slots/Slot 1/Background/Unit Parent/Siege Ram");
                        workshopTargetQueued.SetActive(true);
                        Right_UpdateWorkshopSlot(target.workshop);
                        break;
                    case TroopCore.Type.None:
                        workshopTargetQueued = null;
                        break;
                    default:
                        workshopTargetQueued = null;
                        break;
                }

                //Setup the draggable script
                if (workshopTargetQueued != null)
                {
                    Draggable drag = workshopTargetQueued.GetComponent<Draggable>();
                    drag.unit = target.workshop.ownedUnit.reference;
                }

                /*
                //TRAINED UNITS

                //Initialise units
                for (int unit = 0; unit < 3; unit++)
                {
                    Right_GetUISubmenuBuilding(0).GetChild(1).GetChild(0).GetChild(0).GetChild(unit).gameObject.SetActive(false);
                }

                if (target.workshop.ownedUnit.reference)
                {
                    GameObject workshopTargetTrained;
                    switch (target.workshop.ownedUnit.unit)
                    {
                        case UnitCore.Type.Ambush:
                            workshopTargetTrained = GameObject.Find("Workshop/Trained Units/Slot 1/Unit Parent/Ambush Draggable");
                            workshopTargetTrained.SetActive(true);
                            break;
                        case UnitCore.Type.Assault:
                            workshopTargetTrained = GameObject.Find("Workshop/Trained Units/Slot 1/Unit Parent/Assault Draggable");
                            workshopTargetTrained.SetActive(true);
                            break;
                        case UnitCore.Type.Defender:
                            workshopTargetTrained = GameObject.Find("Workshop/Trained Units/Slot 1/Unit Parent/Defender Draggable");
                            workshopTargetTrained.SetActive(true);
                            break;
                        case UnitCore.Type.None:
                            workshopTargetTrained = null;
                            break;
                        default:
                            workshopTargetTrained = null;
                            break;
                    }
                }
                */

                Centre_RefreshTroopPaths();
                break;
        }
    }

    void Right_ToggleUIBuilding(int target)
    {
        for (int count = 0; count < 3; count++)
        {
            Right_GetUISubmenuBuilding(count).gameObject.SetActive(false);
            CanvasGroup targetToggle = Right_GetUISubmenuBuilding(count).GetComponent<CanvasGroup>();
            targetToggle.blocksRaycasts = false; targetToggle.interactable = false;
        }

        if (target != -1)
        {
            Right_GetUISubmenuBuilding(target).gameObject.SetActive(true);
            CanvasGroup targetToggle = Right_GetUISubmenuBuilding(target).GetComponent<CanvasGroup>();
            targetToggle.blocksRaycasts = true; targetToggle.interactable = true;
        }
        else
        {
            right_SubmenuSelectedText.text = "Select a building...";
        }
    }

    Transform Right_GetUISubmenuBuilding(int element)
    {
        return right_SubmenuT.GetChild(element + 1);
    }

    void Right_UpdateBarracksSlot(BuildingCore.Barracks target)
    {
        Transform fill = Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(1);
        if (target.ownedUnit.generationTime == 0 | target.ownedUnit.generationTime == 16)
        {
            if (target.ownedUnit.queued == TroopCore.Type.None)
            {
                fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                fill.GetComponent<Image>().fillAmount = 0;
            }
            else
            {
                fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Queued";
                fill.GetComponent<Image>().fillAmount = 1;
            }
        }
        else
        {
            fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = target.ownedUnit.generationTime.ToString("F1");
            fill.GetComponent<Image>().fillAmount = target.ownedUnit.generationTime / 15;
        }
    }

    void Right_UpdateBarracksSlot(float generationTime)
    {

        Transform fill = Right_GetUISubmenuBuilding(0).GetChild(0).GetChild(0).GetChild(1);
        if (generationTime == 0 | generationTime == 16)
        {
            if (right_BuildingSelected.barracks.ownedUnit.queued == TroopCore.Type.None)
            {
                fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                fill.GetComponent<Image>().fillAmount = 0;
            }
            else
            {
                fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Queued";
                fill.GetComponent<Image>().fillAmount = 1;
            }
        }
        else
        {
            fill.GetChild(1).GetComponent<TextMeshProUGUI>().text = generationTime.ToString("F1");
            fill.GetComponent<Image>().fillAmount = generationTime / 15;
        }
    }

    public void Right_PingBarracksSlot(BuildingCore sender)
    {
        if (active)
        {
            if (sender = right_BuildingSelected)
            {
                Right_ShowBuildingContents(sender);
                Centre_RefreshTroopPins();
            }
        }
    }

    void Right_UpdateWorkshopSlot(BuildingCore.Workshop target)
    {
        Transform fill = Right_GetUISubmenuBuilding(2).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(1);
        if (target.ownedUnit.generationTime == 0 | target.ownedUnit.generationTime == 16)
        {
            fill.GetComponent<Image>().fillAmount = 0;
            fill.GetChild(0).GetComponent<Image>().color = Color.clear;
            fill.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

            fill.GetComponent<CanvasGroup>().interactable = false;
            fill.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            fill.GetComponent<Image>().fillAmount = target.ownedUnit.generationTime / 15;
            fill.GetChild(0).GetComponent<Image>().color = Color.black;
            fill.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = target.ownedUnit.generationTime.ToString("F1");

            fill.GetComponent<CanvasGroup>().interactable = true;
            fill.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    void Right_UpdateWorkshopSlot(float generationTime)
    {
        Transform fill = Right_GetUISubmenuBuilding(2).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(1);
        if (generationTime == 0 | generationTime == 16)
        {
            fill.GetComponent<Image>().fillAmount = 0;
            fill.GetChild(0).GetComponent<Image>().color = Color.clear;
            fill.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

            fill.GetComponent<CanvasGroup>().interactable = false;
            fill.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            fill.GetComponent<Image>().fillAmount = generationTime / 15;
            fill.GetChild(0).GetComponent<Image>().color = Color.black;
            fill.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = generationTime.ToString("F1");

            fill.GetComponent<CanvasGroup>().interactable = true;
            fill.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void Right_CycleBarracksUnitGeneration()
    {
        //Get Type
        int toQueue = (int)Mathf.Repeat(Helper.ConvertUnitTypeToInt(right_BuildingSelected.barracks.ownedUnit.queued) + 1, 3);

        //Queue this Type
        right_BuildingSelected.barracks.QueueNewUnit(Helper.ConvertUnitIntToType(toQueue));

        //Refresh!
        Right_PingBarracksSlot(right_BuildingSelected);
    }

    public void Right_PingWorkshopSlot(BuildingCore sender)
    {
        if (active)
        {
            if (sender = right_BuildingSelected)
            {
                Right_ShowBuildingContents(sender);
                Centre_RefreshTroopPins();
            }
        }
    }

    public void Right_QueueWorkshopUnit(int unit)
    {
        if (unit >= 3)
        {
            //Get Type
            var toQueue = Helper.ConvertUnitIntToType(unit);

            //Queue this Type
            right_BuildingSelected.workshop.QueueNewUnit(toQueue);

            //Refresh!
            Right_PingWorkshopSlot(right_BuildingSelected);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            active = !active;

            if (active)
            {
                canvas.alpha = 1;
                canvas.interactable = true;
                canvas.blocksRaycasts = true;

                right_BuildingType = -1;

                //Methods
                Right_CreateBuildingList();
                Right_InitialiseCounters();
                Right_ToggleUIBuilding(-1);
                Centre_InitialiseMap(HighLightMode.Lighten);
                Centre_UpdateMapRegions();
                Centre_ClearMapPathing();
                Centre_RefreshTroopPins();

                //Camera
                Camera.main.GetComponent<CameraControl>().offsetMagnitude = 0;
            }
            else
            {
                canvas.alpha = 0;
                canvas.interactable = false;
                canvas.blocksRaycasts = false;

                //Methods
                right_SubmenuBuildingHealthBar.fillAmount = 0;
                right_SubmenuBuildingHealthText.text = "";

                //Camera
                Camera.main.GetComponent<CameraControl>().offsetMagnitude = 6;
            }
        }

        if (active)
        {
            if (right_BuildingSelected)
            {
                if (right_BuildingSelected.type == BuildingCore.Type.Barracks)
                {
                    Right_UpdateBarracksSlot(right_BuildingSelected.barracks.RequestGenerationInformation());
                }
                else if (right_BuildingSelected.type == BuildingCore.Type.Workshop)
                {
                    Right_UpdateWorkshopSlot(right_BuildingSelected.workshop.RequestGenerationInformation());
                }
            }
        }
    }
}