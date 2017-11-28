using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Analytics;
using UnityEngine.AI;

public class Transfer
{
    public float Val, Max, PCT;

    public Transfer() { }

    public Transfer(float val, float max, float pct)
    {
        Val = val;
        Max = max;
        PCT = pct;
    }

    public Transfer(float pct)
    {
        PCT = pct;
    }

    public Transfer(Parameters.Resource target)
    {
        Val = target.Val;
        Max = target.Max;
        PCT = target.PCT();
    }
}

public class Source
{
    public UnitCore causer;
    public Type type;
    public enum Type
    {
        Weapon, Item, Ability, State, Natural, System
    }

    public Item weapon;
    public Item item;
    public State state;

    //Constructors
    public Source(UnitCore ThisCore, Type Type)
    {
        causer = ThisCore;
        type = Type;
    }

    public Source(UnitCore Causer, Item Weapon, WeaponCore Core)
    {
        causer = Causer;
        type = Type.Weapon;
        weapon = Weapon;
    }

    public Source(UnitCore Causer, Item Item)
    {
        causer = Causer;
        type = Type.Item;
        item = Item;
    }

    public Source(UnitCore Causer, State State)
    {
        causer = Causer;
        type = Type.State;
        state = State;
    }
}

[System.Serializable]
public class Bounty
{
    public UnitCore causer;
    public float damage;
}

public class UnitCore : NetworkBehaviour
{
    [Header("SETUP")]
    [SyncVar]
    public int id;
    public Defs.UnitType mode;
    public bool isHostile = true;
    public Transform uiParent;
    public UnitHUD hud;
    public GameObject emptyHolder;
    public Player player;
    public UI_HUD playerHUD;
    public Movement movement;
    public HERO_StateController states;
    public Material hostile;
    public Material friendly;

    [Header("Movement")]
    public bool isMoving;
    public bool isSprinting;
    [SyncVar]
    public bool disallowSprint;
    public float speedMoveBase;
    [SyncVar]
    public float speedMove;
    public float speedSprintBase;
    public float speedSprint;
    public float sprintDrain;
    public float accelerationBase;
    [SyncVar]
    public float acceleration;
    [SyncVar]
    public bool lockRotation;

    [Header("State")]
    public bool isBlocking;

    [Header("Critter")]
    public Transform aggro;
    public NavMeshAgent navMesh;

    [Header("Popups")]
    public GameObject popupObject;
    public Transform popupParent;
    public List<GameObject> popups;
    public float popupSizeMin = 24;
    public float popupSizeMax = 60;

    [Header("Particles")]
    public GameObject bloodSpray;
    public ParticleSystem regenPlus;

    [Header("Fade")]
    [SyncVar]
    public float sinceLoss;

    [Header("Common States")]
    public State overwhelmed;

    [Header("Resource Setup")]
    [SerializeField]
    private SyncListString activeResources = new SyncListString();
    [SyncVar]
    public bool ready;
    [Range(100, 5000)]
    public float baseHealth;
    public float regenerationHealth;
    public float regenerationHealthDelay;
    [SyncVar]
    public bool isPlayerReviving = false;
    public Parameters.Resource Health = new Parameters.Resource();
    [Range(0, 5000)]
    public float baseStamina;
    public float regenerationStamina;
    public Parameters.Resource Stamina = new Parameters.Resource();
    [Range(0, 5000)]
    public float baseMana;
    public float regenerationMana;
    public Parameters.Resource Mana = new Parameters.Resource();
    [Range(0, 5000)]
    public float baseBloodlust;
    public Parameters.Resource Bloodlust = new Parameters.Resource();
    [Range(0, 5000)]
    public float baseSunlight;
    public Parameters.Resource Sunlight = new Parameters.Resource();
    [Range(0, 5000)]
    public float baseMoonlight;
    public Parameters.Resource Moonlight = new Parameters.Resource();
    [Range(0, 100)]
    public float baseCurse;
    public Parameters.Resource Curse = new Parameters.Resource();
    [Range(0, 50)]
    public float baseCorruption;
    public Parameters.Resource Corruption = new Parameters.Resource();
    [Range(0, 100)]
    public float baseDarkness;
    public Parameters.Resource Darkness = new Parameters.Resource();

    [Header("Bounty")]
    public AudioClip audioKill;
    public AudioClip audioDeath;
    public float value;
    public List<Bounty> bounty = new List<Bounty>();

    [Header("Mechanics")]
    public State stateParry;

    void Awake()
    {
        switch (mode)
        {
            case Defs.UnitType.Critter:
                navMesh = GetComponent<NavMeshAgent>();
                break;
            case Defs.UnitType.Troop:
                break;
            case Defs.UnitType.Player:
                player = GetComponent<Player>();
                movement = GetComponent<Movement>();
                states = GetComponent<HERO_StateController>();
                break;
        }
    }

    void OnEnable()
    {
        if (isClient)
        {
            if (mode == Defs.UnitType.Player)
            {
                float s = GetComponent<CharacterController>().radius;
                transform.GetChild(4).localScale = new Vector3(s, s, s);
                if (isLocalPlayer)
                {
                    isHostile = false;
                    transform.GetChild(4).GetComponent<MeshRenderer>().material = friendly;
                }
                else
                {
                    isHostile = true;
                    transform.GetChild(4).GetComponent<MeshRenderer>().material = hostile;
                }
            }
        }
    }

    void Start()
    {
        if (isServer)
        {
            sinceLoss = -1;
            GameStatus.i.ManageUnitCores(this, true);
            id = gameObject.GetHashCode();
            SetupResources();
        }

        //Get References
        popupParent = GameObject.Find("HUD/Overlay/Popups").transform;
        uiParent = GameObject.Find("HUD/Overlay/Unit UI").transform;
        emptyHolder = UI_Styles.i.unitHUD.emptyHolder;

        //Get HUD
        if (mode == Defs.UnitType.Player)
            playerHUD = UI_HUD.i;
    }

    [Server]
    void SetupResources()
    {
        Health = Parameters.i.GetResource(Defs.ResourceTypes.Health, baseHealth);
        activeResources.Add("Health");
        RpcAddResource(Defs.ResourceTypes.Health, Health.Max);

        if (baseStamina != 0)
        {
            Stamina = Parameters.i.GetResource(Defs.ResourceTypes.Stamina, baseStamina);
            activeResources.Add("Stamina");
            RpcAddResource(Defs.ResourceTypes.Stamina, Stamina.Max);
        }
        if (baseMana != 0)
        {
            Mana = Parameters.i.GetResource(Defs.ResourceTypes.Mana, baseMana);
            activeResources.Add("Mana");
            RpcAddResource(Defs.ResourceTypes.Mana, Mana.Max);
        }
        if (baseBloodlust != 0)
        {
            Bloodlust = Parameters.i.GetResource(Defs.ResourceTypes.Bloodlust, baseBloodlust);
            RpcAddResource(Defs.ResourceTypes.Bloodlust, Bloodlust.Max);
        }
        if (baseSunlight != 0)
        {
            Sunlight = Parameters.i.GetResource(Defs.ResourceTypes.Sunlight, baseSunlight);
            RpcAddResource(Defs.ResourceTypes.Sunlight, Sunlight.Max);
        }
        if (baseMoonlight != 0)
        {
            Moonlight = Parameters.i.GetResource(Defs.ResourceTypes.Moonlight, baseMoonlight);
            RpcAddResource(Defs.ResourceTypes.Moonlight, Moonlight.Max);
        }
        if (baseCurse != 0)
        {
            Curse = Parameters.i.GetResource(Defs.ResourceTypes.Curse, baseCurse);
            //RpcAddResource(Defs.ResourceTypes.Curse, Curse.Max);
        }
        if (baseCorruption != 0)
        {
            Corruption = Parameters.i.GetResource(Defs.ResourceTypes.Corruption, baseCorruption);
            //RpcAddResource(Defs.ResourceTypes.Corruption, Corruption.Max);
        }
        if (baseDarkness != 0)
        {
            Darkness = Parameters.i.GetResource(Defs.ResourceTypes.Darkness, baseDarkness);
            RpcAddResource(Defs.ResourceTypes.Darkness, Darkness.Max);
        }
        ready = true;
    }

    /*
    [Command]
    public void CmdRequestInformation()
    {
        string[] copied = new string[activeResources.Count];
        activeResources.CopyTo(copied);
        RpcCreateHUD(copied);
    }

    [ClientRpc]
    void RpcCreateHUD(string[] recieved)
    {
        if (hud == null)
        {
            //yield return new WaitForEndOfFrame();
            GameObject instance = Instantiate(emptyHolder, uiParent);
            instance.name = gameObject.name;
            hud = instance.AddComponent<UnitHUD>();
            hud.Setup(this, recieved, mode, isHostile);
            ready = true;
        }
    }
    */

    void Update()
    {
        ClientUpdate();

        if (!isLocalPlayer)
        {
            return;
        }

        if (tag == "Player" && isLocalPlayer && !UI_Chat.i.active)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Health, -50, false);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Health, 200, false);
            }
        }
    }

    [ClientCallback]
    void ClientUpdate()
    {
        if (hud == null && ready)
        {
            string[] copied = new string[activeResources.Count];
            activeResources.CopyTo(copied, 0);
            CreateHUD(copied);
        }

        if (hud != null)
        {
            if (sinceLoss == -1) { }
            else if (sinceLoss >= 0.75f)
            {
                hud.FadeMode(true);
                sinceLoss = -1;
            }
            else
            {
                sinceLoss += Time.deltaTime;
            }
        }
    }

    [Client]
    void CreateHUD(string[] instruction)
    {
        if (hud == null)
        {
            //yield return new WaitForEndOfFrame();
            GameObject instance = Instantiate(emptyHolder, uiParent);
            instance.name = gameObject.name;
            hud = instance.AddComponent<UnitHUD>();
            hud.Setup(this, instruction, mode, isHostile);
        }
    }

    [Server]
    public void Tick()
    {
        switch (mode)
        {
            case Defs.UnitType.Critter:
                if (aggro != null)
                {
                    navMesh.SetDestination(aggro.position);
                }
                break;
            case Defs.UnitType.Troop:
                break;
            case Defs.UnitType.Player:
                if (isPlayerReviving)
                    return;

                //Regeneration - Health
                if (Health.Val != Health.Max && regenerationHealth != 0)
                    if (regenerationHealthDelay == 0)
                    {
                        ChangeResource(new Source(this, Source.Type.Natural), Defs.ResourceTypes.Health, regenerationHealth * Clock.tickLength, true);
                        regenPlus.Emit(Mathf.Clamp(Mathf.RoundToInt((regenerationHealth * 0.5f) * Clock.tickLength), 1, 10));
                    }
                    else
                    {
                        regenerationHealthDelay = Mathf.Clamp(regenerationHealthDelay - Clock.tickLength, 0, 6);
                    }

                //Regeneration - Stamina
                if (Stamina.Val != Stamina.Max && regenerationHealth != 0 && !isSprinting && !isBlocking)
                {
                    if (isMoving)
                    {
                        ChangeResource(new Source(this, Source.Type.Natural), Defs.ResourceTypes.Stamina, regenerationStamina * Clock.tickLength, true);
                    }
                    else
                    {
                        ChangeResource(new Source(this, Source.Type.Natural), Defs.ResourceTypes.Stamina, regenerationStamina * 0.375f, true);

                    }
                }

                //Regeneration - Mana
                if (Mana.Val != Mana.Max && regenerationMana != 0)
                {
                    ChangeResource(new Source(this, Source.Type.Natural), Defs.ResourceTypes.Mana, regenerationMana * Clock.tickLength, true);
                }

                if (mode == Defs.UnitType.Player)
                {
                    CheckSprintRequirements();
                }
                break;
        }
    }

    [Server]
    void CheckSprintRequirements()
    {
        if (!disallowSprint)
        {
            movement.staminaCheck = Stamina.Val >= Mathf.Abs(sprintDrain) * Clock.tickLength;
        }
        else
        {
            movement.staminaCheck = false;
        }
    }

    [Command]
    public void CmdAlterMoving(bool to)
    {
        isMoving = to;
    }

    [Command]
    public void CmdAlterSprinting(bool to)
    {
        isSprinting = to;
    }

    [ClientRpc]
    void RpcAddResource(Defs.ResourceTypes target, float max)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.AddResource(target, max);
        }
    }

    [ClientRpc]
    void RpcUpdateResource(Defs.ResourceTypes target, Transfer vals)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.UpdateResource(target, vals);
        }
    }

    [Server]
    public void MeleeStrike(bool to)
    {
        if (to)
        {
            lockRotation = true;
        }
        else
        {
            lockRotation = false;
        }
    }

    [ClientRpc]
    public void RpcMeleeRecover()
    {
        movement.acceleratedSpeed = 0;
    }


    [Server]
    public void ProjectileWind(bool to)
    {
        if (to)
        {
            speedMove = speedMoveBase - 1.5f;
            disallowSprint = true;
        }
        else
        {
            speedMove = speedMoveBase;
            disallowSprint = false;
        }

        //Sprint Check
        CheckSprintRequirements();
    }

    [Server]
    public void ShieldActive(bool to)
    {
        if (to)
        {
            speedMove = speedMoveBase - 0.5f;
            acceleration = accelerationBase / 2;
            isBlocking = true;
        }
        else
        {
            speedMove = speedMoveBase;
            acceleration = accelerationBase;
            isBlocking = false;
        }

        //Sprint Check
        CheckSprintRequirements();
    }

    /*
    [Server]
    public void CmdIsWinding(bool to, bool allowSprint, bool resetAcceleration)
    {
        if (to)
        {
            speedMove = 3.5f;
            disallowSprint = allowSprint;
            CheckSprintRequirements();
        }
        else
        {
            speedMove = 5f;
            disallowSprint = allowSprint;
            CheckSprintRequirements();
        }

        if (resetAcceleration)
        {
            movement.acceleratedSpeed = 0;
        }
    }
    */

    [Command]
    public void CmdChangeResource(Source.Type type, Defs.ResourceTypes target, float amount, bool skipPopups)
    {
        ChangeResource(new Source(this, type), target, amount, skipPopups);
    }

    [Server]
    public void ChangeResource(Source source, Defs.ResourceTypes target, float amount, bool skipPopups)
    {
        if (amount != 0)
        {
            bool isNegative = false;
            if (amount < 0)
            {
                sinceLoss = 0;
                hud.FadeMode(false);
                isNegative = true;
            }

            switch (target)
            {
                case Defs.ResourceTypes.Health:
                    //PRE PROCESSING
                    bool negate = false;
                    switch (source.type)
                    {
                        case Source.Type.Weapon:

                            #region PARRY
                            if (source.weapon.WeaponType == Defs.WeaponType.Melee)
                            {
                                if (states != null)
                                {
                                    if (states.HasState(stateParry))
                                    {
                                        if (isNegative)
                                        {
                                            negate = true;
                                            RpcCreatePopupText("Parried!", Color.grey);
                                        }
                                    }
                                }
                            }
                            #endregion

                            break;
                        case Source.Type.Item:
                            break;
                        case Source.Type.State:
                            break;
                        case Source.Type.Natural:
                            break;
                        case Source.Type.System:
                            break;
                    }

                    if (negate)
                        return;

                    //Apply Amount
                    Health.Val = Mathf.Clamp(Health.Val + amount, 0, Health.Max);

                    switch (mode)
                    {
                        case Defs.UnitType.Critter:
                            aggro = source.causer.transform;
                            break;
                        case Defs.UnitType.Troop:
                            break;
                        case Defs.UnitType.Player:
                            break;
                    }

                    //Bounty
                    if (source.causer != this)
                    {
                        if (isNegative)
                        {
                            //Damage
                            bool exists = false;
                            for (int count = 0; count < bounty.Count; count++)
                            {
                                if (bounty[count].causer == source.causer)
                                {
                                    bounty[count].damage += Mathf.Abs(amount);
                                    exists = true;
                                    break;
                                }
                            }

                            if (!exists)
                            {
                                Bounty newBounty = new Bounty()
                                {
                                    causer = source.causer,
                                    damage = Mathf.Abs(amount),
                                };
                                bounty.Add(newBounty);
                            }
                        }
                    }

                    //Popups
                    if (Health.Popups && !skipPopups)
                    {
                        Color to = Health.Color;
                        if (isNegative)
                            to = Color.red;

                        if (Mathf.Abs(amount) < 5000)
                        {
                            RpcCreatePopup(Mathf.Abs(amount), Health.PCT(amount), to);
                        }
                        else
                        {
                            int num = Random.Range(0, 7);
                            string text = "Kek lmao, the programmer can't maths";
                            switch (num)
                            {
                                case 0:
                                    text = "Tons of damage!";
                                    break;
                                case 1:
                                    text = "I am fed";
                                    break;
                                case 2:
                                    text = "Holy S*** run from me!";
                                    break;
                                case 3:
                                    text = "I am a balanced character";
                                    break;
                                case 4:
                                    text = "The definition of OP";
                                    break;
                                case 5:
                                    text = "My presence has been made clear";
                                    break;
                                case 6:
                                    text = "This is your queue to leave";
                                    break;
                            }
                            RpcCreatePopupText(text, Color.red);
                        }
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Health, new Transfer(Health));

                    //Effects
                    if (isNegative)
                    {
                        //Effects
                        RpcEffects(Health.PCT(Mathf.Abs(amount)));
                    }

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {

                        //Delay Regen
                        if (isNegative)
                            regenerationHealthDelay = 6;

                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Health, new Transfer(Health));
                    }

                    if (Health.Val == 0)
                    {
                        StartCoroutine(Die(source.causer));
                    }
                    break;
                case Defs.ResourceTypes.Stamina:
                    //Apply Amount
                    Stamina.Val = Mathf.Clamp(Stamina.Val + amount, 0, Stamina.Max);

                    //Popups
                    if (Stamina.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Stamina.PCT(amount), Stamina.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Stamina, new Transfer(Stamina));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Stamina, new Transfer(Stamina));
                    }
                    break;
                case Defs.ResourceTypes.Mana:
                    //Apply Amount
                    Mana.Val = Mathf.Clamp(Mana.Val + amount, 0, Mana.Max);

                    //Popups
                    if (Mana.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Mana.PCT(amount), Mana.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Mana, new Transfer(Mana));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Mana, new Transfer(Mana));
                    }
                    break;
                case Defs.ResourceTypes.Bloodlust:
                    //Apply Amount
                    Bloodlust.Val = Mathf.Clamp(Bloodlust.Val + amount, 0, Bloodlust.Max);

                    //Popups
                    if (Bloodlust.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Bloodlust.PCT(amount), Bloodlust.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Bloodlust, new Transfer(Bloodlust));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Bloodlust, new Transfer(Bloodlust));
                    }
                    break;
                case Defs.ResourceTypes.Sunlight:
                    //Apply Amount
                    Sunlight.Val = Mathf.Clamp(Sunlight.Val + amount, 0, Sunlight.Max);

                    //Popups
                    if (Sunlight.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Sunlight.PCT(amount), Sunlight.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Sunlight, new Transfer(Sunlight));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Sunlight, new Transfer(Sunlight));
                    }
                    break;
                case Defs.ResourceTypes.Moonlight:
                    //Apply Amount
                    Moonlight.Val = Mathf.Clamp(Moonlight.Val + amount, 0, Moonlight.Max);

                    //Popups
                    if (Moonlight.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Moonlight.PCT(amount), Moonlight.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Moonlight, new Transfer(Moonlight));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Moonlight, new Transfer(Moonlight));
                    }
                    break;
                case Defs.ResourceTypes.Curse:
                    //Apply Amount
                    Curse.Val = Mathf.Clamp(Curse.Val + amount, 0, Curse.Max);

                    //Popups
                    if (Curse.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Curse.PCT(amount), Curse.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Curse, new Transfer(Curse));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Curse, new Transfer(Curse));
                    }
                    break;
                case Defs.ResourceTypes.Corruption:
                    //Apply Amount
                    Corruption.Val = Mathf.Clamp(Corruption.Val + amount, 0, Corruption.Max);

                    //Popups
                    if (Corruption.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Corruption.PCT(amount), Corruption.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Corruption, new Transfer(Corruption));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Corruption, new Transfer(Corruption));
                    }
                    break;
                case Defs.ResourceTypes.Darkness:
                    //Apply Amount
                    Darkness.Val = Mathf.Clamp(Darkness.Val + amount, 0, Darkness.Max);

                    //Popups
                    if (Darkness.Popups && !skipPopups)
                    {
                        RpcCreatePopup(amount, Darkness.PCT(amount), Darkness.Color);
                    }

                    //HUD
                    RpcUpdateHUD(Defs.ResourceTypes.Darkness, new Transfer(Darkness));

                    //Player Specific
                    if (mode == Defs.UnitType.Player)
                    {
                        //HUD
                        RpcUpdateResource(Defs.ResourceTypes.Darkness, new Transfer(Darkness));
                    }
                    break;
            }
        }
        else
        {
            //Debug.LogError("CHANGE RESOURCE: Passed a value of 0! :" + target.ToString());
        }
    }


    [ClientRpc]
    void RpcEffects(float positivePCT)
    {
        if (isLocalPlayer)
        {
            CameraShaker.Instance.ShakeOnce(2, 2, 0, 0.5f);
        }
        ParticleSystem blood = Instantiate(bloodSpray, new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), transform.position.z), Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0)).GetComponent<ParticleSystem>();
        blood.Emit(Mathf.FloorToInt(80 * positivePCT));
        Destroy(blood.gameObject, 1.75f);
    }

    [ClientRpc]
    void RpcUpdateHUD(Defs.ResourceTypes target, Transfer pack)
    {
        hud.Refresh(target, pack);
    }

    /*
    [Space(20)]
    [Header("HEALTH")]
    [SyncVar]
    public float health_Val;
    [SyncVar]
    public float health_Max;
#pragma warning disable IDE1006 // Naming Styles
    public float health_PCT()
#pragma warning restore IDE1006 // Naming Styles
    {
        return health_Val / health_Max;
    }
#pragma warning disable IDE1006 // Naming Styles
    public float health_PCTValue(float PCT)
#pragma warning restore IDE1006 // Naming Styles
    {
        return PCT * health_Max;
    }

    [Header("Regeneration")]
    [SyncVar]
    public float health_Regen;
    [SyncVar]
    public float health_RegenDelay;
    [SyncVar]
    public float health_RegenTimer;

    [Command]
    public void CmdHealth_Change(float amount, bool noRotation, bool overrideLogRules)
    {
        bool negative = false;
        bool isDead = false;
        if (amount != 0)
        {
            health_Val = Mathf.Clamp(health_Val + amount, 0, health_Max);

            if (amount < 0)
            {
                health_RegenTimer = health_RegenDelay;
                negative = true;
                if (mode == Defs.UnitType.Player && amount < 20)
                {
                    RpcEffects(Mathf.Abs(amount) / health_Max);
                }
            }

            if (health_Val == 0)
            {
                isDead = true;
            }
            else
            {
                if (!overrideLogRules)
                {
                    if (amount > 20)
                    {
                        Debug.Log(name + " @" + Time.time + " : + <color=green>" + amount + "</color> Health");
                    }
                    else if (amount < -20)
                    {
                        Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(amount) + "</color> Health");
                    }
                }
                else
                {
                    if (amount > 0)
                    {
                        Debug.Log(name + " @" + Time.time + " : + <color=green>" + amount + "</color> Health");
                    }
                    else
                    {
                        Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(amount) + "</color> Health");
                    }
                }
            }

            //Create Popups on criteria
            //if (amount < 0 || amount > 20)
            //RpcCreatePopup(amount, (amount / health_Max), (noRotation) ? new Quaternion() : lastHitRotation, isDead);

            //Now update the HUD
            CmdHUDUpdateHealth(health_Val, health_Max, negative);

        }
        else
        {
            Debug.LogWarning(name + " @" + Time.time + " : Called [Change] method but passed 0 as changing amount");
        }
    }

    [Command]
    public void CmdHealth_ChangeByPCT(float percentage, bool noRotation, bool overrideLogRules)
    {
        bool negative = false;
        bool isDead = false;
        if (percentage != 0)
        {
            float amount;
            amount = health_Max * percentage;
            health_Val = Mathf.Clamp(health_Val + amount, 0, health_Max);

            if (amount < 0)
            {
                health_RegenTimer = health_RegenDelay;
                negative = true;
                if (mode == Defs.UnitType.Player && amount < 20)
                {
                    RpcEffects(Mathf.Abs(percentage));
                }
            }

            if (health_Val == 0)
            {
                isDead = true;
            }
            else
            {
                if (!overrideLogRules)
                {
                    if (amount > 20)
                    {
                        Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Health");
                    }
                    else if (amount < -20)
                    {
                        Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(percentage) + "</color>% (" + amount + ") Health");
                    }
                }
                else
                {
                    if (amount > 0)
                    {
                        Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Health");
                    }
                    else
                    {
                        Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(percentage) + "</color>% (" + amount + ") Health");
                    }
                }

                if (amount == 0)
                {
                    Debug.LogWarning(name + " @" + Time.time + " : Called [ChangeByPCT] method but passed 0 as changing amount");
                }
            }

            //Create Popups on criteria
            //if (amount < 0 || amount > 20)
            //RpcCreatePopup(amount, (amount / health_Max), (noRotation) ? new Quaternion() : lastHitRotation, isDead);

            //Now update the HUD
            CmdHUDUpdateHealth(health_Val, health_Max, negative);
        }
    }

        */
    /*
    [Space(20)]
    [Header("STAMINA")]
    [SyncVar]
    public float stamina_Val;
    [SyncVar]
    public float stamina_Max;
#pragma warning disable IDE1006 // Naming Styles
    public float stamina_PCT()
#pragma warning restore IDE1006 // Naming Styles
    {
        return stamina_Val / stamina_Max;
    }
#pragma warning disable IDE1006 // Naming Styles
    public float stamina_PCTValue(float PCT)
#pragma warning restore IDE1006 // Naming Styles
    {
        return PCT * stamina_Max;
    }

    [Header("Regeneration")]
    [SyncVar]
    public float stamina_Regen;

    [Command]
    public void CmdStamina_Change(float amount, bool overrideLogRules)
    {
        bool negative = false;
        if (amount != 0)
        {
            stamina_Val = Mathf.Clamp(stamina_Val + amount, 0, stamina_Max);

            if (amount < 0)
            {
                negative = true;
            }

            if (!overrideLogRules)
            {
                if (amount > 20)
                {
                    Debug.Log(name + " @" + Time.time + " : + <color=green>" + amount + "</color> Stamina");
                }
                else if (amount < -20)
                {
                    Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(amount) + "</color> Stamina");
                }
            }
            else
            {
                if (amount > 0)
                {
                    Debug.Log(name + " @" + Time.time + " : + <color=green>" + amount + "</color> Stamina");
                }
                else
                {
                    Debug.Log(name + " @" + Time.time + " : - <color=red>" + Mathf.Abs(amount) + "</color> Stamina");
                }
            }

            //Now update the HUD
            CmdHUDUpdateStamina(stamina_Val, stamina_Max, negative);

            //Refresh HUD
            //RpcServerUpdateHUD();
            //hud.Refresh(this);

            //Check if Refresh PlayerHUD
            /*
            if (mode == UnitHUD.Mode.Player)
            {
                playerHUD.UpdateStamina(stamina_PCT(), stamina_Val, stamina_Max);
            }
            */
    /*
}
else
{
    Debug.LogWarning(name + " @" + Time.time + " : Called [Change] method but passed 0 as changing amount");
}
}

[Command]
public void CmdStamina_ChangeByPCT(float percentage, bool overrideLogRules)
{
if (percentage != 0)
{
    float amount;
    amount = stamina_Max * percentage;
    stamina_Val = Mathf.Clamp(stamina_Val + amount, 0, stamina_Max);

    if (amount < 0)
    {
        hud.FadeMode(false);
        sinceLoss = 0;
    }

    if (!overrideLogRules)
    {
        if (amount > 20)
        {
            Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Stamina");
        }
        else if (amount < -20)
        {
            Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Stamina");
        }
    }
    else
    {
        if (amount > 0)
        {
            Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Stamina");
        }
        else
        {
            Debug.Log(name + " @" + Time.time + " : + <color=green>" + percentage + "</color>% (" + amount + ") Stamina");
        }
    }

    if (amount == 0)
    {
        Debug.LogWarning(name + " @" + Time.time + " : Called [ChangeByPCT] method but passed 0 as changing amount");
    }

    //Refresh HUD
    //RpcServerUpdateHUD();
    //hud.Refresh(this);

    //Check if Refresh PlayerHUD
    /*
    if (mode == UnitHUD.Mode.Player)
    {
        playerHUD.UpdateStamina(stamina_PCT(), stamina_Val, stamina_Max);
    }
    */
    /*
}
}

[Command]
void CmdHUDUpdateHealth(float val, float max, bool negative)
{
Transfer pack = new Transfer()
{
    Val = val,
    Max = health_Max,
    PCT = val / health_Max
};
RpcHUDUpdateHealth(pack, negative);
}

[ClientRpc]
void RpcHUDUpdateHealth(Transfer pack, bool negative)
{
hud.Refresh(pack, Defs.ResourceTypes.Health);

if (negative)
{
    sinceLoss = 0;
    hud.FadeMode(false);
}

if (isLocalPlayer)
{
    if (mode == Defs.UnitType.Player)
    {
        //playerHUD.UpdateHealth(pack);
    }
}
}

[Command]
void CmdHUDUpdateStamina(float val, float max, bool negative)
{
Transfer pack = new Transfer()
{
    Val = val,
    Max = stamina_Max,
    PCT = val / stamina_Max
};
RpcHUDUpdateStamina(pack, negative);
}

[ClientRpc]
void RpcHUDUpdateStamina(Transfer pack, bool negative)
{
hud.Refresh(pack, Defs.ResourceTypes.Stamina);

if (negative)
{
    sinceLoss = 0;
    hud.FadeMode(false);
}

if (isLocalPlayer)
{
    if (mode == Defs.UnitType.Player)
    {
        //playerHUD.UpdateStamina(pack);
    }
}
}
*/

    [Client]
    public void ChangeVisibility(bool to)
    {
        if (mode != Defs.UnitType.Player)
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().enabled = to;
        }
        else
        {
            transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = to;
        }

        //HUD
        hud.ChangeVisiblity(to);
    }

    [ClientRpc]
    void RpcCreatePopup(float amount, float percentage, Color color)
    {
        percentage = Mathf.Abs(percentage);
        GameObject newPopup = Instantiate(popupObject, popupParent, false);
        popups.Add(newPopup);
        newPopup.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(-35, 35), 0));
        newPopup.name = "Popup @" + Time.time + " (OF " + amount + ")";
        TextMeshProUGUI newText = newPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        newText.transform.rotation = Quaternion.Euler(0, 0, 0);

        newText.text = amount.ToString("F0");
        newText.color = color;
        newText.fontSize = Mathf.Clamp(((popupSizeMax - popupSizeMin) * percentage) + popupSizeMin, popupSizeMin, popupSizeMax);

        PopupController targetPopup = newPopup.GetComponent<PopupController>();
        targetPopup.target = transform;
    }

    [ClientRpc]
    void RpcCreatePopupText(string text, Color color)
    {
        GameObject newPopup = Instantiate(popupObject, popupParent, false);
        popups.Add(newPopup);
        newPopup.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(-35, 35), 0));
        newPopup.name = "Popup @" + Time.time + " (OF " + text + ")";
        TextMeshProUGUI newText = newPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        newText.transform.rotation = Quaternion.Euler(0, 0, 0);

        newText.text = text;
        newText.color = color;
        newText.fontSize = 28;

        PopupController targetPopup = newPopup.GetComponent<PopupController>();
        targetPopup.target = transform;
    }

    [Server]
    public void RunEffector(Source source, Effector instructions, bool isBlocked)
    {
        if (isPlayerReviving)
            return;

        Effector copied = new Effector()
        {
            health = instructions.health,
            healthPCT = instructions.healthPCT,
            stamina = instructions.stamina,
            staminaPCT = instructions.staminaPCT,
            mana = instructions.mana,
            manaPCT = instructions.manaPCT,
            bloodlust = instructions.bloodlust,
            sunlight = instructions.sunlight,
            moonlight = instructions.moonlight,
            curse = instructions.curse,
            corruption = instructions.corruption,
            darkness = instructions.darkness,
        };

        if (isBlocked)
        {
            copied.stamina = -(Mathf.Abs((copied.health * 0.1f)) + (5 * states.GetNumberOfIndependentStates(overwhelmed)));
            copied.health = 0;

            Debug.Log((copied.health * 0.1f) + (5 * states.GetNumberOfIndependentStates(overwhelmed)));
            //Now add a stack of overwhelmed
            states.AddState(source.causer, overwhelmed);
        }

        if (copied.health != 0)
            ChangeResource(source, Defs.ResourceTypes.Health, copied.health, false);
        else if (isBlocked)
            RpcCreatePopupText("Blocked!", Color.gray);
        if (copied.healthPCT != 0)
            ChangeResource(source, Defs.ResourceTypes.Health, Health.Max * copied.healthPCT, false);
        if (copied.stamina != 0)
            ChangeResource(source, Defs.ResourceTypes.Stamina, copied.stamina, false);
        if (copied.staminaPCT != 0)
            ChangeResource(source, Defs.ResourceTypes.Stamina, Stamina.Max * copied.staminaPCT, false);
        if (copied.mana != 0)
            ChangeResource(source, Defs.ResourceTypes.Mana, copied.mana, false);
        if (copied.manaPCT != 0)
            ChangeResource(source, Defs.ResourceTypes.Mana, Mana.Max * copied.manaPCT, false);
        if (copied.bloodlust != 0)
            ChangeResource(source, Defs.ResourceTypes.Bloodlust, copied.bloodlust, false);
        if (copied.sunlight != 0)
            ChangeResource(source, Defs.ResourceTypes.Sunlight, copied.sunlight, false);
        if (copied.moonlight != 0)
            ChangeResource(source, Defs.ResourceTypes.Moonlight, copied.moonlight, false);
        if (copied.curse != 0)
            ChangeResource(source, Defs.ResourceTypes.Curse, copied.curse, false);
        if (copied.corruption != 0)
            ChangeResource(source, Defs.ResourceTypes.Corruption, copied.corruption, false);
        if (copied.darkness != 0)
            ChangeResource(source, Defs.ResourceTypes.Darkness, copied.darkness, false);
    }

    IEnumerator Die(UnitCore killer)
    {
        yield return new WaitForEndOfFrame();

        //Remove from database
        if (mode != Defs.UnitType.Player)
            GameStatus.i.ManageUnitCores(this, false);

        //Give XP
        float totalDamage = 0;
        for (int count = 0; count < bounty.Count; count++)
        {
            totalDamage += bounty[count].damage;
        }

        for (int count = 0; count < bounty.Count; count++)
        {
            if (bounty[count].causer.GetComponent<PlayerCore>() != null)
            {
                Defs.XpType type;
                switch (mode)
                {
                    case Defs.UnitType.Critter:
                        type = Defs.XpType.Neutral;
                        break;
                    case Defs.UnitType.Troop:
                        type = Defs.XpType.Enemy;
                        break;
                    case Defs.UnitType.Player:
                        type = Defs.XpType.Enemy;
                        break;
                    default:
                        type = Defs.XpType.Neutral;
                        break;
                }
                float xpPCT = bounty[count].damage / totalDamage;
                float xpVal = value * xpPCT;
                bounty[count].causer.GetComponent<PlayerCore>().XpChange(type, xpVal);

                //Log this
                Debug.Log(bounty[count].causer.GetComponent<Player>().steamName + " has gained xp: " + xpVal + " (" + (xpPCT * 100).ToString("F1") + "%)");
            }
        }

        //Victim
        switch (mode)
        {
            case Defs.UnitType.Critter:
                GameObject.Find("Regions").transform.GetChild(GetComponent<CritterCore>().masterRegion).GetComponent<RegionCore>().ReportCritterDeath(gameObject);
                NetworkServer.Destroy(gameObject);
                break;
            case Defs.UnitType.Troop:
                break;
            case Defs.UnitType.Player:
                //Killer
                if (killer.mode == Defs.UnitType.Player)
                {
                    killer.RpcKill(player.steamName);
                }
                RpcDie();
                GetComponent<PlayerCore>().respawnTimer = 61;
                GetComponent<PlayerCore>().RpcDie();
                isPlayerReviving = true;
                break;
        }
    }

    [ClientRpc]
    void RpcKill(string steamName)
    {
        if (isLocalPlayer)
        {
            UI_Message.i.Show("You have slain " + steamName, Defs.MessageType.Notify);
            AudioController.i.Play2DSound(audioKill, 1, 1, 1);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int count = 0; count < players.Length; count++)
            {
                if (players[count].GetComponent<Player>().steamName == steamName)
                {
                    Debug.Log("Did the deed");
                    TimeManager.i.StartSlowMotion(players[count].GetComponent<UnitCore>());
                }
            }
        }
    }

    [ClientRpc]
    void RpcDie()
    {
        if (isLocalPlayer)
        {
            AudioController.i.Play2DSound(audioDeath, 1, 1, 1);
            CameraShaker.Instance.ShakeOnce(10, 14, 0, 1);
        }
    }

    [Server]
    public void Revive()
    {
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Health, Health.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Stamina, Stamina.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Mana, Mana.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Bloodlust, Bloodlust.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Sunlight, Sunlight.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Moonlight, Moonlight.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Curse, Curse.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Corruption, Corruption.Max, true);
        CmdChangeResource(Source.Type.System, Defs.ResourceTypes.Darkness, Darkness.Max, true);
    }
}