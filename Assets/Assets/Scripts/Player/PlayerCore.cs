using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerCore : NetworkBehaviour
{
    [Header("References")]
    HERO_MusicController music;
    public UnitCore unitCore;
    public PlayerInventory inv;
    public WeaponCore weaponCore;

    [Header("Identification")]
    [SyncVar]
    public int playerID;
    /*
    public int identifier;
    */

    [Header("Class")]
    public Defs.PlayerClass playerClass;
    public int skillPoints = 1;
    public Vector3 availableSlots = Vector3.zero;
    public Vector3 usedSlots = Vector3.zero;

    public static Vector3 PlayerClassToSlots(Defs.PlayerClass playerClass)
    {
        int redSlots = 0;
        int blueSlots = 0;
        int greenSlots = 0;

        //Slots
        switch (playerClass)
        {
            case Defs.PlayerClass.Warrior:
                redSlots = 3;
                blueSlots = 1;
                greenSlots = 1;
                break;
            case Defs.PlayerClass.Commander:
                redSlots = 1;
                blueSlots = 3;
                greenSlots = 1;
                break;
            case Defs.PlayerClass.Scout:
                redSlots = 1;
                blueSlots = 1;
                greenSlots = 3;
                break;
            case Defs.PlayerClass.Colonel:
                redSlots = 2;
                blueSlots = 2;
                greenSlots = 1;
                break;
            case Defs.PlayerClass.Monk:
                redSlots = 2;
                blueSlots = 1;
                greenSlots = 2;
                break;
            case Defs.PlayerClass.Herald:
                redSlots = 1;
                blueSlots = 2;
                greenSlots = 2;
                break;
            case Defs.PlayerClass.Knight:
                redSlots = 3;
                blueSlots = 2;
                greenSlots = 0;
                break;
            case Defs.PlayerClass.Assassin:
                redSlots = 3;
                blueSlots = 0;
                greenSlots = 2;
                break;
            case Defs.PlayerClass.Sentinel:
                redSlots = 2;
                blueSlots = 3;
                greenSlots = 0;
                break;
            case Defs.PlayerClass.Strategist:
                redSlots = 0;
                blueSlots = 3;
                greenSlots = 2;
                break;
            case Defs.PlayerClass.Patrol:
                redSlots = 0;
                blueSlots = 2;
                greenSlots = 3;
                break;
            case Defs.PlayerClass.Hunter:
                redSlots = 2;
                blueSlots = 0;
                greenSlots = 3;
                break;
        }
        return new Vector3(redSlots, blueSlots, greenSlots);
    }

    [Header("Level")]
    public int level = 1;
    public float xpNeutral;
    public float xpEnemy;
    public float xpTotal;
    public float xpMax = 700;
    public float xpBase;
    public float XpToNextLevel()
    {
        return xpMax - xpTotal;
    }
    public float XpPCT()
    {
        return xpMax / (xpTotal - xpBase);
    }

    [Header("Parameters")]
    [SyncVar]
    public int parameterTotalStats = 100;
    public float parameterStrength;
    public float parameterStrengthPerma;
    public float parameterStrengthInfluence = 0.5f;
    public float parameterAgility;
    public float parameterAggression;
    public int parameters_Goldies;

    [Header("Weight")]
    public float weight_Val;
    public float weight_Max;

    [Header("Abilities")]
    public Transform actionBarT;
    public Abilities abilitySheet;

    [System.Serializable]
    public class EquippedAbilities
    {
        public bool equipped;
        public Ability ability;
        public float cooldown;
    }
    public List<EquippedAbilities> equippedAbilities = new List<EquippedAbilities>(5);

    public List<KeyCode> abilityKeys = new List<KeyCode>(5);

    [Header("HotBar")]
    public List<PlayerInventory.InventoryItem> items = new List<PlayerInventory.InventoryItem>(10);
    public List<KeyCode> itemKeys = new List<KeyCode>(10);
    public int usageItem;
    public float usageDelayBase;
    public float usageDelay;

    [Header("Mount")]
    [SyncVar]
    public bool mounted;
    [SyncVar]
    public bool mounting;
    [SyncVar]
    public float callTime = 0;
    public CanvasGroup mountGroup;
    public Image mountBar;

    [Header("Location")]
    [SyncVar]
    public string currentLocation;
    public RegionCore currentRegion;
    public Transform regionMaster;
    public List<RegionCore> regions;
    public List<RegionCore> ownedRegions;
    public BuildingList ownedBuildings;

    [Header("Respawn")]
    public int respawnTimer;
    public ParticleSystem respawnParticles;

    [Server]
    public void RpcIdentify(int ID)
    {
        playerID = ID;
    }

    void OnEnable()
    {
        music = GameObject.Find("GameController").GetComponent<HERO_MusicController>();
        unitCore = GetComponent<UnitCore>();
        inv = GetComponent<PlayerInventory>();
        weaponCore = GetComponent<WeaponCore>();

        regionMaster = GameObject.Find("Regions").transform;

        //Ability
        actionBarT = GameObject.Find("UI/HUD/ActionBar").transform;

        if (isClient)
        {
            for (int count = 0; count < 5; count++)
            {
                abilityKeys.Add(Binds.i.GetBind("Skill " + (count + 1)).key);
            }

            for (int count = 0; count < 10; count++)
            {
                itemKeys.Add(Binds.i.GetBind("Item " + (count + 1)).key);
            }
        }

        //Mount
        mountGroup = GameObject.Find("UI/HUD/ChannelBack").GetComponent<CanvasGroup>();
        mountBar = GameObject.Find("UI/HUD/ChannelBack/ChannelBarBack/ChannelBar").GetComponent<Image>();

        //Now pass all required values
        if (isLocalPlayer)
        {
            UI_HUD.i.target = GetComponent<UnitCore>();
            UI_HUD.i.playerCore = UI_CharacterMenu.i.playerCore = this;
        }

        if (isServer)
        {
            availableSlots = PlayerClassToSlots(playerClass);

            for (int count = 0; count < 5; count++)
            {
                if (!equippedAbilities[count].equipped)
                {
                    RpcHUDPingAbility(count, Defs.AbilityMode.Level);
                }
            }

            int[] skills = new int[5];
            string[] keys = new string[5];
            for (int count = 0; count < 5; count++)
            {
                keys[count] = abilityKeys[count].ToString();
                skills[count] = -1;
            }
            RpcActionBar_Rebuild(skills, keys);
        }
    }

    void Update()
    {
        if (unitCore.isPlayerReviving)
            return;

        if (isServer)
        {
            ServerUpdate();
        }

        if (isClient)
        {
            ClientUpdate();
        }
    }

    [Command]
    void CmdMount(bool to)
    {
        mounting = to;
    }

    [Server]
    void ServerUpdate()
    {
        if (!mounted)
        {
            if (mounting)
            {
                if (callTime == 3)
                {
                    mounting = false;
                    mounted = true;
                    callTime = 0;
                    RpcAdaptMount(true);
                    RpcUpdateChannel(0, 0);
                }
                else
                {
                    callTime = Mathf.Clamp(callTime + Time.deltaTime, 0, 3);
                    RpcUpdateChannel(callTime / 3, 1);
                }
            }
        }
        else
        {
            if (mounting)
            {
                mounting = false;
                mounted = false;
                callTime = 0;
                RpcAdaptMount(false);
            }
        }

        //Ability Cooldowns
        for (int count = 0; count < equippedAbilities.Count; count++)
        {
            if (equippedAbilities[count].cooldown != -1)
            {
                equippedAbilities[count].cooldown = Mathf.Clamp(equippedAbilities[count].cooldown - Time.deltaTime, 0, 15);

                if (equippedAbilities[count].cooldown == 0)
                {
                    //HUD
                    RpcHUDPingAbility(count, Defs.AbilityMode.Ready);
                    equippedAbilities[count].cooldown = -1;
                }
            }
        }

        //Item Usage
        if (usageDelay != 0 && usageItem != -1)
        {
            usageDelay = Mathf.Clamp(usageDelay - Time.deltaTime, 0, 100);
            weaponCore.RpcUpdateItemUsageHUD(usageDelay / usageDelayBase, EffectorMethods.CheckIfEnoughResources(unitCore, items[usageItem].item.CostEffector));

            if (usageDelay == 0)
                UseDelayedItem(usageItem);
        }

        //Create New Method - Need Inventory, Level up systems

        //Calculations - Strength:
        parameterStrength = Mathf.Clamp(parameterStrengthPerma + (parameterTotalStats * parameterStrengthInfluence), 0, parameterTotalStats * 2);

        //Calculations - Agility:
        parameterAgility = Mathf.Clamp(parameterTotalStats - parameterStrength, 0, parameterTotalStats * 2);

        //WEIGHT

        //Use an update function later - Need Inventory
        weight_Max = Mathf.Clamp(parameterAgility * 0.9f, 0, parameterTotalStats * 2);

        //For the time being Add all weighing objects into this equation
        weight_Val = Mathf.Clamp((parameters_Goldies * 0.3f), 0, weight_Max);

    }

    [ClientRpc]
    void RpcAdaptMount(bool to)
    {
        if (isLocalPlayer)
        {
            GetComponent<Movement>().isMounted = to;
        }
    }

    [ClientRpc]
    void RpcUpdateChannel(float PCT, float alpha)
    {
        if (isLocalPlayer)
        {
            mountBar.fillAmount = PCT;
            mountGroup.alpha = alpha;
        }
    }

    void ClientUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Abilities
        if (!UI_Chat.i.active)
        {
            for (int count = 0; count < 5; count++)
            {
                if (Input.GetKeyDown(abilityKeys[count]))
                {
                    CmdTakeAbilityInput(count);
                }
            }
        }

        //Hotbar
        if (!UI_Chat.i.active)
        {
            for (int count = 0; count < 10; count++)
            {
                if (Input.GetKeyDown(itemKeys[count]))
                {
                    CmdCancelItem(true);
                    CmdTakeItemInput(count);
                }
            }
        }

        //Mounting
        if (Input.GetKeyDown(Binds.i.GetBind("Mount").key))
        {
            if (!mounted)
            {
                CmdMount(true);
            }
            else
            {
                CmdMount(true);
            }
        }
    }


    [Command]
    public void CmdTakeAbilityInput(int ability)
    {
        ActionBar_UseAbility(ability);
    }

    [Command]
    public void CmdTakeItemInput(int item)
    {
        if ((item + 1) <= items.Count && (weaponCore.hands[0].status != WeaponCore.Wield.Status.Wind && weaponCore.hands[0].status != WeaponCore.Wield.Status.Active && weaponCore.hands[1].status != WeaponCore.Wield.Status.Wind && weaponCore.hands[1].status != WeaponCore.Wield.Status.Active))
        {
            HotBar_UseItem(item);
        }
    }

    [Server]
    public void XpChange(Defs.XpType type, float positiveAmount)
    {
        //Add XP
        switch (type)
        {
            case Defs.XpType.Neutral:
                xpNeutral += positiveAmount;
                break;
            case Defs.XpType.Enemy:
                xpEnemy += positiveAmount;
                break;
        }

        //Calculate Total
        xpTotal = (xpNeutral + xpEnemy);
        parameterAggression = xpEnemy / xpTotal;

        //Check for level up
        if (xpTotal >= xpMax)
        {
            Level_Up();
        }
    }

    [Server]
    void Level_Up()
    {
        if (level < 5)
        {
            //Add Total Stats
            float pastStats = parameterTotalStats;

            float newTotalStats = parameterTotalStats * (1.1f - (0.01f * (level - 1)));
            parameterTotalStats = (int)Mathf.Round(newTotalStats);
            float totalStatsDifference = newTotalStats - pastStats;

            Debug.Log(totalStatsDifference);

            //Add Permanent Stats
            float pastStrengthPerma = parameterStrengthPerma;

            float percent = ((parameterStrength + weight_Val) * 100) / pastStats;
            parameterStrengthPerma += totalStatsDifference * (percent / 100);

            float permaDifference = parameterStrengthPerma - pastStrengthPerma;

            //Debug.Log("Player Gained " + permaDifference + " Strength!");

            //Add one Level
            xpBase = xpMax;
            level++;

            //Give Skill Points
            skillPoints++;

            RpcChangeSPADisplay(true);

            switch (level)
            {
                case 2:
                    xpMax = 1800;
                    break;
                case 3:
                    xpMax = 3300;
                    break;
                case 4:
                    xpMax = 4800;
                    break;
            }

            //Log Event
            Debug.Log("PLAYER CORE: " + GetComponent<Player>().steamName + " is now Level " + level);

            //Check if another level up is necessary
            if (xpTotal >= xpMax)
            {
                Level_Up();
            }

            //Now Flash empty slots
            for (int count = 0; count < 5; count++)
            {
                if (!equippedAbilities[count].equipped)
                {
                    RpcHUDPingAbility(count, Defs.AbilityMode.Level);
                }
            }
        }
        else
        {
            //Log Event
            Debug.LogError("PLAYER CORE: " + GetComponent<Player>().steamName + " is already at max level!");
        }
    }

    /// <summary>
    /// Use this to see if the player has enough money for something
    /// </summary>
    /// <param name="amount">Compare this amount</param>
    /// <returns></returns>
    public bool CompareMoney(int amount)
    {
        if (parameters_Goldies - amount < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Use this to see if the Player has enough weight for something
    /// </summary>
    /// <param name="weight">Amount to be compared</param>
    /// <returns></returns>
    public bool CompareWeight(float weight)
    {
        if (weight_Val + weight >= weight_Max)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [ServerCallback]
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "RegionEnter")
        {
            if (currentRegion != null && currentRegion.playersInRegion.Contains(this))
            {
                if (currentRegion.capturer == playerID && currentRegion.owner != playerID)
                {
                    RpcPingCaptureTimer(true);
                }
                currentRegion.playersInRegion.Remove(this);
            }

            RegionCore thisRegionCore = collision.transform.parent.GetComponent<RegionCore>();
            currentLocation = thisRegionCore.regionHandle;
            currentRegion = thisRegionCore;
            RpcPingLocation(thisRegionCore.owner, thisRegionCore.nonCore, thisRegionCore.regionHandle);
            if (!thisRegionCore.playersInRegion.Contains(this))
                thisRegionCore.playersInRegion.Add(this);

            if (!thisRegionCore.nonCore)
                thisRegionCore.SpawnCritters();

            //Update Music
            //music.UpdateTrack(Helper.ConvertRegionNameToID(collision.transform.parent.name));

            //Log event
            Debug.Log("Player has moved to " + currentLocation);

            //Update Fog Of War
            //UpdateFogOfWar();
        }
    }

    [ClientRpc]
    void RpcPingLocation(int owner, bool nonCore, string regionHandle)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.UpdateLocation(owner, nonCore, regionHandle, playerID);

        }
    }

    [ClientRpc]
    void RpcPingCaptureTimer(bool fail)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.PingCaptureTimer(fail);
        }
    }

    [ServerCallback]
    void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "RegionEnter")
        {
            if (currentRegion != null)
            {
                collision.transform.parent.GetComponent<RegionCore>().CmdHandleCapture(playerID);
            }
        }
    }

    /*
    [ServerCallback]
    public List<RegionCore> GetOwnedRegions()
    {
        List<RegionCore> regions = Helper.GetAllRegionCores();
        List<RegionCore> tester = new List<RegionCore>();
        for (int count = 0; count < regions.Count; count++)
        {
            if (regions[count].owner == playerID)
            {
                tester.Add(regions[count]);
            }
        }

        ownedRegions = tester;
        return tester;
    }
    */

    [ServerCallback]
    void FindOwnedBuildings()
    {
        for (int count = 0; count < 12; count++)
        {
            //Debug.Log(Helper.GetRegionCore(count));
            //Helper.GetRegionCore(count).CheckRegionStatus();
            regions[count].CheckRegionStatus();

            if (regions[count].nonCore && regions[count].owner == playerID)
            {
                if (regions[count].ownedBuildings.Count != 0)
                {
                    for (int building = 0; building < regions[count].ownedBuildings.Count; building++)
                    {
                        switch (regions[count].ownedBuildings[building].type)
                        {
                            case BuildingCore.Type.Barracks:
                                ownedBuildings.Barracks.Add(regions[count].ownedBuildings[building]);
                                break;
                            case BuildingCore.Type.Granary:
                                ownedBuildings.Granary.Add(regions[count].ownedBuildings[building]);
                                break;
                            case BuildingCore.Type.Workshop:
                                ownedBuildings.Workshop.Add(regions[count].ownedBuildings[building]);
                                break;
                        }
                    }
                }
            }
        }
        //Sort each list
        /*
        newList.Barracks.Sort();
        newList.Granary.Sort();
        newList.Workshop.Sort();
        */
    }

    [ServerCallback]
    public BuildingList GetOwnedBuildings()
    {
        ownedBuildings.Barracks.Clear();
        ownedBuildings.Granary.Clear();
        ownedBuildings.Workshop.Clear();

        FindOwnedBuildings();
        return ownedBuildings;
    }

    [System.Serializable]
    public class BuildingList
    {
        public List<BuildingCore> Barracks;
        public List<BuildingCore> Granary;
        public List<BuildingCore> Workshop;
    }

    /*
    public void UpdateFogOfWar()
    {
        List<GameObject> regions = Helper.GetAllRegionObjects();
        for (int count = 0; count < 12; count++)
        {
            regions[count].GetComponent<FogOfWar>().ChangeVisiblity(this);
        }
    }
    */

    #region Character Menu
    //CALLBACKS
    [Command]
    public void CmdMenuRequestAbilitiesInfo()
    {
        RpcMenuCallbackAbilitiesInfo(skillPoints, level, xpTotal, xpMax, playerClass, usedSlots);
    }

    [ClientRpc]
    public void RpcMenuCallbackAbilitiesInfo(int retrievedSkillpoints, int level, float xp, float xpMax, Defs.PlayerClass retrievedClass, Vector3 retrievedUsedSlots)
    {
        if (isLocalPlayer)
        {
            UI_CharacterMenu.i.MenuCallbackAbilitiesInfo(retrievedSkillpoints, level, xp, xpMax, retrievedClass, retrievedUsedSlots);
        }
    }
    #endregion

    #region Abilities Menu
    //CALLBACKS
    [Command]
    public void CmdAbilitiesRequestMenu()
    {
        //Conditions
        if (skillPoints == 0)
        {
            Debug.Log("Has no skill points");
            return;
        }

        //Find Learned Abilities
        RpcCallbackAbilityMenu(playerClass, usedSlots, AbilityGetUnavailable(), AbilityGetEquipped());
    }

    [ClientRpc]
    void RpcCallbackAbilityMenu(Defs.PlayerClass retrievedClass, Vector3 retrievedUsedSlots, bool[] availableAbilities, int[] equippedAbilities)
    {
        if (isLocalPlayer)
        {
            UI_CharacterMenu.i.AbilityCallbackMenu(retrievedClass, retrievedUsedSlots, availableAbilities, equippedAbilities);
        }
    }

    [Command]
    public void CmdAssignAbility(int ability, int slot)
    {
        if (skillPoints == 0)
            return;

        switch (abilitySheet.contained[ability].slotType)
        {
            case Defs.SlotType.Red:
                if (availableSlots.x == 0)
                {
                    Debug.Log("Has not got enough slots!");
                    return;
                }
                break;
            case Defs.SlotType.Blue:
                if (availableSlots.y == 0)
                {
                    Debug.Log("Has not got enough slots!");
                    return;
                }
                break;
            case Defs.SlotType.Green:
                if (availableSlots.z == 0)
                {
                    Debug.Log("Has not got enough slots!");
                    return;
                }
                break;
        }

        if (!equippedAbilities[slot].equipped)
        {
            if (!AbilityIsLearned(ability))
            {
                EquippedAbilities newAbility = new EquippedAbilities()
                {
                    ability = abilitySheet.contained[ability],
                    cooldown = 0,
                    equipped = true,
                };
                equippedAbilities[slot] = newAbility;

                int[] skills = new int[5];
                for (int count = 0; count < skills.Length; count++)
                {
                    skills[count] = (equippedAbilities[count].equipped) ? abilitySheet.contained.IndexOf(equippedAbilities[count].ability) : -1;
                }
                string[] keys = new string[5];
                for (int count = 0; count < skills.Length; count++)
                {
                    keys[count] = abilityKeys[count].ToString();
                }

                //Remove Skill Points
                skillPoints = Mathf.Clamp(skillPoints - 1, 0, 6);
                switch (abilitySheet.contained[ability].slotType)
                {
                    case Defs.SlotType.Red:
                        usedSlots.x++;
                        availableSlots.x--;
                        break;
                    case Defs.SlotType.Blue:
                        usedSlots.y++;
                        availableSlots.y--;
                        break;
                    case Defs.SlotType.Green:
                        usedSlots.z++;
                        availableSlots.z--;
                        break;
                }

                //HUD
                RpcActionBar_Rebuild(skills, keys);
                RpcHUDPingAbility(slot, Defs.AbilityMode.Chosen);

                if (skillPoints == 0)
                {
                    for (int count = 0; count < equippedAbilities.Count; count++)
                    {
                        if (count != slot)
                            RpcHUDPingAbility(count, Defs.AbilityMode.NotChosen);
                    }
                    RpcCloseAbilityMenu();
                    RpcChangeSPADisplay(false);
                }
                else
                {
                    RpcCallbackAbilityMenu(playerClass, usedSlots, AbilityGetUnavailable(), AbilityGetEquipped());
                }
            }
            else
                Debug.Log("Already learned this ability!");
        }
        else
            Debug.Log("There is already an ability in that slot!");
    }

    [ClientRpc]
    void RpcActionBar_Rebuild(int[] id, string[] controls)
    {
        if (isLocalPlayer)
            UI_HUD.i.UpdateActionBar(id, controls);
    }

    [ClientRpc]
    void RpcCloseAbilityMenu()
    {
        if (isLocalPlayer)
            UI_CharacterMenu.i.AbilityMenuClose();
    }

    [ClientRpc]
    void RpcChangeSPADisplay(bool to)
    {
        if (isLocalPlayer)
            UI_HUD.i.ChangeSPADisplay(to);
    }

    [Server]
    public void ActionBar_UseAbility(int slot)
    {
        if (!equippedAbilities[slot].equipped)
            return;

        if (equippedAbilities[slot].cooldown != -1)
        {
            //HUD
            RpcHUDPingAbility(slot, Defs.AbilityMode.Cooldown);
            return;
        }

        if (EffectorMethods.CheckIfEnoughResources(unitCore, equippedAbilities[slot].ability.cost))
        {
            for (int action = 0; action < equippedAbilities[slot].ability.actions.Count; action++)
            {
                //Target
                UnitCore target;
                switch (equippedAbilities[slot].ability.actions[action].target)
                {
                    case Ability.Action.Target.Self:
                        target = unitCore;
                        break;
                }

                //Type
                switch (equippedAbilities[slot].ability.actions[action].type)
                {
                    case Ability.Action.Type.ApplyState:
                        GetComponent<HERO_StateController>().AddStates(unitCore, equippedAbilities[slot].ability.actions[action].targetStates);
                        break;
                    case Ability.Action.Type.RemoveState:
                        GetComponent<HERO_StateController>().RemoveStates(equippedAbilities[slot].ability.actions[action].targetStates);
                        break;
                    case Ability.Action.Type.ApplyEffector:
                        unitCore.RunEffector(new Source(unitCore, Source.Type.Ability), equippedAbilities[slot].ability.actions[action].targetEffector, false);
                        break;
                    case Ability.Action.Type.Animation:
                        GetComponent<Animator>().SetTrigger(equippedAbilities[slot].ability.actions[action].animationTrigger);
                        break;
                }

                //Movement
                bool cancel = false;
                Quaternion targetDirection = transform.rotation;
                switch (equippedAbilities[slot].ability.actions[action].movementType)
                {
                    case Ability.Action.MovementType.None:
                        cancel = true;
                        break;
                    case Ability.Action.MovementType.MoveDirection:
                        Vector3 face = new Vector3(transform.position.x + Input.GetAxis("Horizontal"), transform.position.y, transform.position.z + Input.GetAxis("Vertical"));
                        transform.LookAt(face);
                        targetDirection = transform.rotation;
                        break;
                    case Ability.Action.MovementType.MouseDirection:
                        targetDirection = transform.rotation;
                        break;
                    case Ability.Action.MovementType.CustomDirection:
                        targetDirection = equippedAbilities[slot].ability.actions[action].direction;
                        break;
                }

                if (!cancel)
                {
                    StartCoroutine(MoveInDirection(targetDirection, equippedAbilities[slot].ability.actions[action].distance, equippedAbilities[slot].ability.actions[action].time));
                }
            }

            //Cooldown
            equippedAbilities[slot].cooldown = 15;

            //Unit HUD
            RpcPingAbility(abilitySheet.contained.IndexOf(equippedAbilities[slot].ability));

            //HUD
            RpcHUDPingAbility(slot, Defs.AbilityMode.Use);
        }
        else
        {
            //HUD
            RpcHUDPingAbility(slot, Defs.AbilityMode.Cooldown);
        }
    }

    IEnumerator MoveInDirection(Quaternion direction, float distance, float time)
    {
        transform.rotation = Quaternion.Euler(0, direction.eulerAngles.y, 0);
        float speed = distance / time;
        float traversal = 0;
        while (traversal < distance)
        {
            GetComponent<CharacterController>().SimpleMove(transform.forward * speed);
            traversal += speed;
            Debug.Log(traversal + " and " + distance + " and " + speed);
            yield return new WaitForEndOfFrame();
        }
    }

    //UTILITY
    [Server]
    bool AbilityIsSlotFilled(int slot)
    {
        return equippedAbilities[slot].equipped;
    }

    [Server]
    bool[] AbilityGetUnavailable()
    {
        bool[] fail = new bool[abilitySheet.contained.Count];
        for (int ability = 0; ability < abilitySheet.contained.Count; ability++)
        {
            if (AbilityIsLearned(ability))
            {
                fail[ability] = true;
            }

            if (!AbilityHasEnoughSlots(ability))
            {
                fail[ability] = true;
            }
        }
        return fail;
    }

    [Server]
    bool AbilityHasEnoughSlots(int ability)
    {
        switch (abilitySheet.contained[ability].slotType)
        {
            case Defs.SlotType.Red:
                if (availableSlots.x > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case Defs.SlotType.Blue:
                if (availableSlots.y > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case Defs.SlotType.Green:
                if (availableSlots.z > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                Debug.LogError("Ability is none of the setup types!");
                return false;
        }
    }

    [Server]
    bool AbilityIsLearned(int ability)
    {
        for (int count = 0; count < equippedAbilities.Count; count++)
        {
            if (equippedAbilities[count].equipped)
            {
                if (equippedAbilities[count].ability == abilitySheet.contained[ability])
                    return true;
            }
        }
        return false;
    }

    [Server]
    int[] AbilityGetEquipped()
    {
        int[] equipped = new int[equippedAbilities.Count];
        for (int count = 0; count < equippedAbilities.Count; count++)
        {
            if (!equippedAbilities[count].equipped)
            {
                equipped[count] = -1;
            }
            else
            {
                equipped[count] = abilitySheet.contained.IndexOf(equippedAbilities[count].ability);
            }
        }
        return equipped;
    }

    [ClientRpc]
    void RpcPingAbility(int ability)
    {
        //Unit HUD
        StartCoroutine(unitCore.hud.PingAbility(ability));
        unitCore.hud.FadeMode(false);
    }

    [ClientRpc]
    void RpcHUDPingAbility(int ability, Defs.AbilityMode mode)
    {
        if (isLocalPlayer)
            UI_HUD.i.UpdateAbility(ability, mode);
    }
    #endregion

    [Server]
    void HotBar_UseItem(int item)
    {
        if (items[item].item.UsageDelay == 0)
        {
            if (EffectorMethods.CheckIfEnoughResources(unitCore, items[item].item.CostEffector))
            {
                //Apply Effect
                unitCore.RunEffector(new Source(unitCore, items[item].item), items[item].item.SelfEffector, false);

                if (items[item].item.IsProjectileWeapon)
                {
                    weaponCore.SpawnProjectile(items[item].item);
                }

                //Take Costs
                unitCore.RunEffector(new Source(unitCore, items[item].item), items[item].item.CostEffector, false);
                inv.RemoveItem(items[item].item.ID, 1);
            }
            else
            {
                Debug.Log("Not Enough Resource to cast!");
            }
        }
        else
        {
            //Queue Usage
            weaponCore.IsUsingItem(true, false);
            usageItem = item;
            usageDelayBase = usageDelay = items[item].item.UsageDelay;
        }
    }

    [Server]
    public void UseDelayedItem(int item)
    {
        if (EffectorMethods.CheckIfEnoughResources(unitCore, items[item].item.CostEffector))
        {
            //Apply Effect
            unitCore.RunEffector(new Source(unitCore, items[item].item), items[item].item.SelfEffector, false);

            if (items[item].item.IsProjectileWeapon)
            {
                weaponCore.SpawnProjectile(items[item].item);
            }

            //Take Costs
            unitCore.RunEffector(new Source(unitCore, items[item].item), items[item].item.CostEffector, false);
            inv.RemoveItem(items[item].item.ID, 1);
        }
        else
        {
            Debug.Log("Not Enough Resource to use!");
        }

        //Reset
        weaponCore.IsUsingItem(false, false);
        usageItem = -1;
        usageDelay = -1;
    }

    [Command]
    public void CmdCancelItem(bool soft)
    {
        //Reset
        weaponCore.IsUsingItem(false, soft);
        usageItem = -1;
        usageDelay = -1;
    }

    [ClientRpc]
    public void RpcDie()
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.InitiateDeathTimer();
        }
    }

    [Server]
    public void Tick()
    {
        respawnTimer = Mathf.Clamp(respawnTimer - 1, 0, 60);

        //HUD
        if (respawnTimer % 4 == 0)
        {
            RpcUpdateDeathTimer(respawnTimer);
        }

        if (respawnTimer == 0)
        {
            Revive();
        }
    }

    [ClientRpc]
    public void RpcUpdateDeathTimer(int timer)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.UpdateDeathTimer(timer / 4);
        }
    }

    [Server]
    void Revive()
    {
        unitCore.isPlayerReviving = false;
        unitCore.Revive();
        RpcRevive();
    }

    [ClientRpc]
    void RpcRevive()
    {
        if (isLocalPlayer)
        {
            transform.position = new Vector3(50, 210, 50);
            UI_HUD.i.EndDeathTimer();
            respawnParticles.Emit(1000);
        }
    }
}