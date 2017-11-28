using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WeaponCore : NetworkBehaviour
{
    [System.Serializable]
    public class Wield
    {
        public bool empty = true;
        public Item equipped;
        public Melee meleeController;

        public enum Status
        {
            Ready, Wind, StartCancel, Cancel, Active, Strike, Recover
        }
        public Status status;

        public float time;
    }

    //References
    Animator anim;
    [HideInInspector]
    public UnitCore unitCore;
    [HideInInspector]
    public PlayerCore playerCore;
    [HideInInspector]
    public HERO_StateController states;

    [Header("Masks")]
    public LayerMask shieldTargetLayer;

    [Header("Status")]
    public bool isUsingItem;
    public int activeBlockAngle;
    public Pose pose = Pose.Idle;
    public enum Pose
    {
        Idle, Wind, Active, Strike
    }

    [Header("Wielding")]
    public Wield[] hands = new Wield[2];
    public bool[] input = new bool[2];

    [Header("HUD")]
    public Image HUDPointer;
    public Image HUDBackground;
    public Image[] HUDBar = new Image[2];
    public CanvasGroup HUDItemBackground;
    public Image HUDItemBar;

    [Header("Audio")]
    public AudioClip clipHit;

    [Header("Mechanics")]
    public State stateCharge;

    void Start()
    {
        anim = GetComponent<Animator>();
        unitCore = GetComponent<UnitCore>();
        playerCore = GetComponent<PlayerCore>();
        states = GetComponent<HERO_StateController>();

        if (isLocalPlayer && isClient)
        {
            //HUD
            HUDPointer = GameObject.Find("HUD/WeaponCore").GetComponent<Image>();
            HUDBackground = HUDPointer.transform.GetChild(0).GetComponent<Image>();
            HUDBar[0] = HUDBackground.transform.GetChild(0).GetComponent<Image>();
            HUDBar[1] = HUDBackground.transform.GetChild(1).GetComponent<Image>();
            HUDItemBackground = HUDPointer.transform.GetChild(1).GetComponent<CanvasGroup>();
            HUDItemBar = HUDItemBackground.transform.GetChild(0).GetComponent<Image>();

            //Cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        //Test Weapons
        EquipWeapon(MasterListDatabase.i.FetchItem(0), 0);
        EquipWeapon(MasterListDatabase.i.FetchItem(2), 1);
    }

    #region Equipping 
    void EquipWeapon(Item target, int targetHand)
    {

        //Equip Code
        GameObject weapon;
        if (targetHand == 0)
        {
            //Right Hand
            weapon = Instantiate(target.WeaponObject, transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0), false);
        }
        else
        {
            //Left Hand
            weapon = Instantiate(target.WeaponObject, transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0), false);
        }

        if (!isServer)
            return;

        //Now Equip In Code
        hands[targetHand].equipped = target;
        hands[targetHand].empty = false;
        switch (target.WeaponType)
        {
            case Defs.WeaponType.Melee:
                Physics.IgnoreCollision(weapon.GetComponent<Collider>(), GetComponent<Collider>());
                hands[targetHand].meleeController = weapon.AddComponent<Melee>();
                hands[targetHand].meleeController.caller = this;
                hands[targetHand].meleeController.hand = targetHand;
                hands[targetHand].meleeController.itemID = target.ID;
                break;
            case Defs.WeaponType.Projectile:
                break;
            case Defs.WeaponType.Shield:
                break;
        }
    }

    [Server]
    void UnequipWeapon(int targetHand)
    {
        if (!hands[targetHand].empty)
        {
            //Unequip Code
        }
        else
        {
            Debug.LogWarning("Nothing Equipped in that Hand!");
        }
    }
    #endregion

    void Update()
    {
        ServerUpdate();

        if (!isLocalPlayer)
            return;

        if (unitCore.isPlayerReviving)
        {
            input[0] = false;
            input[1] = false;
            return;
        }

        ClientUpdate();
    }

    [ClientCallback]
    void ClientUpdate()
    {
        if (!UI_State.isInUI)
        {
            //Cursor Position
            HUDPointer.rectTransform.position = Input.mousePosition;

            for (int count = 0; count < 2; count++)
            {
                bool to = false;
                if (Input.GetMouseButton(count))
                {
                    to = true;
                    playerCore.CmdCancelItem(false);
                }
                CmdToggleInput(count, to);
            }
        }
    }

    [Client]
    public void IsInUI(bool to)
    {
        if (!to)
        {
            HUDPointer.gameObject.SetActive(true);
        }
        else
        {
            HUDPointer.gameObject.SetActive(false);
            CmdToggleInput(0, false);
            CmdToggleInput(1, false);
            playerCore.CmdCancelItem(false);
        }
    }

    [Command]
    void CmdToggleInput(int hand, bool to)
    {
        input[hand] = to;
    }

    [ServerCallback]
    void ServerUpdate()
    {
        for (int count = 0; count < 2; count++)
        {
            if (!hands[count].empty)
            {
                if (input[count] && !isUsingItem && EffectorMethods.CheckIfEnoughResources(unitCore, hands[count].equipped.CostEffector))
                {
                    switch (hands[count].status)
                    {
                        case Wield.Status.Ready:
                            int check = 0;
                            if (count == 0)
                            {
                                check = 1;
                            }
                            if ((hands[check].status != Wield.Status.Wind && hands[check].status != Wield.Status.Strike) || hands[check].empty)
                            {
                                StartedWind(count);
                                hands[count].status = Wield.Status.Wind;
                            }
                            break;
                        case Wield.Status.Wind:
                            Wind(count);
                            break;
                        case Wield.Status.StartCancel:
                            hands[count].status = Wield.Status.Cancel;
                            break;
                        case Wield.Status.Cancel:
                            Cancel(count);
                            break;
                        case Wield.Status.Active:
                            Active(count);
                            break;
                        case Wield.Status.Strike:
                            Strike(count);
                            break;
                        case Wield.Status.Recover:
                            Recover(count);
                            break;
                    }
                }
                else
                {
                    switch (hands[count].status)
                    {
                        case Wield.Status.Ready:
                            break;
                        case Wield.Status.Wind:
                            hands[count].status = Wield.Status.StartCancel;
                            break;
                        case Wield.Status.StartCancel:
                            StartedCancel(count);
                            hands[count].status = Wield.Status.Cancel;
                            break;
                        case Wield.Status.Cancel:
                            Cancel(count);
                            break;
                        case Wield.Status.Active:
                            hands[count].status = Wield.Status.Ready;
                            StartedReady(count);
                            break;
                        case Wield.Status.Strike:
                            Strike(count);
                            break;
                        case Wield.Status.Recover:
                            Recover(count);
                            break;
                    }
                }
            }
        }
    }

    #region Wind
    [Server]
    void StartedWind(int targetHand)
    {
        RpcControlAnimator(targetHand, Pose.Wind, hands[targetHand].equipped.WeaponType);
        AudioController.i.CmdPlay3DItemSound(playerCore.currentLocation, hands[targetHand].equipped.ID, Defs.ItemSound.WindupSound, transform.position, 0.8f, 10, 30, 0.85f, 1.15f);

        switch (hands[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                break;
            case Defs.WeaponType.Projectile:
                //Conditions
                unitCore.ProjectileWind(true);
                break;
            case Defs.WeaponType.Shield:
                break;
        }
    }

    [Server]
    void Wind(int targetHand)
    {
        float multiplication = 1;
        if (states.HasState(stateCharge))
            multiplication = 2;

        hands[targetHand].time = Mathf.Clamp(hands[targetHand].time + (Time.deltaTime * multiplication), 0, hands[targetHand].equipped.WindupTime);
        RpcUpdateHUD(Wield.Status.Wind, targetHand, hands[targetHand].time / hands[targetHand].equipped.WindupTime);

        if (hands[targetHand].time == hands[targetHand].equipped.WindupTime)
        {
            if (hands[targetHand].equipped.WeaponType != Defs.WeaponType.Shield)
            {
                hands[targetHand].time = hands[targetHand].equipped.StrikeTime;
                StartedStrike(targetHand);
                hands[targetHand].status = Wield.Status.Strike;
                RpcUpdateHUDImmediate(Wield.Status.Strike, targetHand, 1);
            }
            else
            {
                StartedActive(targetHand);
                hands[targetHand].status = Wield.Status.Active;
                RpcUpdateHUDImmediate(Wield.Status.Active, targetHand, 1);
            }
        }
    }
    #endregion

    #region Cancel
    [Server]
    void StartedCancel(int targetHand)
    {
        RpcDirectAnimator(Pose.Idle);

        switch (hands[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                break;
            case Defs.WeaponType.Projectile:
                unitCore.ProjectileWind(false);
                break;
            case Defs.WeaponType.Shield:
                break;
        }
    }

    [Server]
    void Cancel(int targetHand)
    {
        hands[targetHand].time = Mathf.Clamp(hands[targetHand].time - Time.deltaTime, 0, hands[targetHand].equipped.WindupTime);
        RpcUpdateHUD(Wield.Status.Cancel, targetHand, hands[targetHand].time / hands[targetHand].equipped.WindupTime);

        if (hands[targetHand].time == 0)
        {
            hands[targetHand].status = Wield.Status.Ready;
            RpcUpdateHUDImmediate(Wield.Status.Ready, targetHand, 0);
        }
    }
    #endregion

    #region Strike
    [Server]
    void StartedStrike(int targetHand)
    {
        if (states.HasState(stateCharge))
            states.RemoveState(stateCharge);

        //Audio
        AudioController.i.CmdPlay3DItemSound(playerCore.currentLocation, hands[targetHand].equipped.ID, Defs.ItemSound.StrikeSound, transform.position, 0.8f, 10, 30, 0.85f, 1.15f);

        //Take Cost Effectors
        unitCore.RunEffector(new Source(unitCore, hands[targetHand].equipped), hands[targetHand].equipped.CostEffector, false);

        switch (hands[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                Slash(targetHand);

                //Conditions
                unitCore.MeleeStrike(true);
                break;
            case Defs.WeaponType.Projectile:
                RpcControlAnimator(targetHand, Pose.Strike, Defs.WeaponType.Projectile);
                SpawnProjectile(hands[targetHand].equipped);

                //Conditions
                unitCore.ProjectileWind(false);
                break;
            case Defs.WeaponType.Shield:
                Debug.LogError("Trying to Strike with Shield!");
                break;
        }
    }

    [Server]
    void Strike(int targetHand)
    {
        hands[targetHand].time = Mathf.Clamp(hands[targetHand].time - Time.deltaTime, 0, hands[targetHand].equipped.StrikeTime);
        RpcUpdateHUD(Wield.Status.Strike, targetHand, hands[targetHand].time / hands[targetHand].equipped.StrikeTime);

        if (hands[targetHand].time == 0)
        {
            hands[targetHand].time = hands[targetHand].equipped.RecoveryTime;
            StartedRecover(targetHand);
            hands[targetHand].status = Wield.Status.Recover;
            RpcUpdateHUDImmediate(Wield.Status.Ready, targetHand, 0);
        }
    }
    #endregion

    #region Active
    [Server]
    void StartedActive(int targetHand)
    {
        //Take Costs
        unitCore.RunEffector(new Source(unitCore, hands[targetHand].equipped), hands[targetHand].equipped.CostEffector, false);

        hands[targetHand].time = 0;
        unitCore.ShieldActive(true);
        activeBlockAngle = hands[targetHand].equipped.BlockAngle;
    }

    [Server]
    void Active(int targetHand)
    {
        FindTargetsInShieldArea(targetHand);
    }
    #endregion

    #region Recover
    [Server]
    void StartedRecover(int targetHand)
    {
        //Type Specific
        switch (hands[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                hands[targetHand].meleeController.active = false;

                //Conditions
                unitCore.MeleeStrike(false);
                unitCore.RpcMeleeRecover();
                break;
            case Defs.WeaponType.Projectile:
                break;
            case Defs.WeaponType.Shield:
                break;
        }

        //Reset to idle
        RpcDirectAnimator(Pose.Idle);
    }

    [Server]
    void Recover(int targetHand)
    {
        hands[targetHand].time = Mathf.Clamp(hands[targetHand].time - Time.deltaTime, 0, hands[targetHand].equipped.RecoveryTime);
        RpcUpdateHUD(Wield.Status.Recover, targetHand, hands[targetHand].time / hands[targetHand].equipped.RecoveryTime);

        if (hands[targetHand].time == 0)
        {
            StartedReady(targetHand);
            hands[targetHand].status = Wield.Status.Ready;
            RpcUpdateHUDImmediate(Wield.Status.Ready, targetHand, 0);
        }
    }
    #endregion

    #region Ready
    [Server]
    void StartedReady(int targetHand)
    {
        switch (hands[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                break;
            case Defs.WeaponType.Projectile:
                break;
            case Defs.WeaponType.Shield:
                //Conditions
                unitCore.ShieldActive(false);
                activeBlockAngle = 0;

                //HUD
                RpcUpdateHUDImmediate(Wield.Status.Ready, targetHand, 0);
                break;
        }
    }
    #endregion

    #region Actions
    [Server]
    void Slash(int targetHand)
    {
        hands[targetHand].meleeController.active = true;
        hands[targetHand].meleeController.effector = hands[targetHand].equipped.TargetEffector;
        //hands[targetHand].meleeController.instantiateRotation = unitCore.transform.rotation;
        hands[targetHand].meleeController.hitTargets.Clear();
        RpcControlAnimator(targetHand, Pose.Strike, Defs.WeaponType.Melee);
    }

    [Server]
    public void SpawnProjectile(Item projectileWeapon)
    {
        //Spawn Object
        GameObject go = Instantiate(projectileWeapon.ProjectileObject, new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), Quaternion.Euler(-90, transform.rotation.eulerAngles.y + 180, 0));

        //Prevent hitting user
        Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());

        //Add force
        go.GetComponent<Rigidbody>().AddForce(transform.forward * projectileWeapon.Speed, ForceMode.VelocityChange);
        Projectile controller = go.GetComponent<Projectile>();
        //controller.callerID = id;
        controller.caller = this;
        controller.itemID = projectileWeapon.ID;
        controller.effector = projectileWeapon.TargetEffector;
        controller.targetLimit = controller.targetsHit = projectileWeapon.TargetLimit;
        controller.instantiateRotation = unitCore.transform.rotation;
        controller.lifespan = projectileWeapon.Range / projectileWeapon.Speed;
        NetworkServer.Spawn(go);
    }

    [Server]
    void FindTargetsInShieldArea(int targetHand)
    {
        int shieldAngle = hands[targetHand].equipped.BlockAngle;
        Collider[] projectilesInRadius = Physics.OverlapSphere(transform.position, 1.25f, shieldTargetLayer);
        for (int count = 0; count < projectilesInRadius.Length; count++)
        {
            Transform target = projectilesInRadius[count].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < shieldAngle / 2)
            {
                if (target.tag == "Projectile")
                {
                    target.GetComponent<Projectile>().RegisterBlock(gameObject);
                }
            }
        }
    }

    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    #region HUD
    [ClientRpc]
    void RpcUpdateHUD(Wield.Status status, int targetHand, float value)
    {
        if (!isLocalPlayer)
            return;

        HUDBar[targetHand].fillAmount = Mathf.Lerp(HUDBar[targetHand].fillAmount, value, 36 * Time.deltaTime);
        SetHUDColor(status, targetHand);
    }

    [ClientRpc]
    void RpcUpdateHUDImmediate(Wield.Status status, int targetHand, float value)
    {
        if (!isLocalPlayer)
            return;

        HUDBar[targetHand].fillAmount = value;
        SetHUDColor(status, targetHand);
    }

    [Client]
    void SetHUDColor(Wield.Status status, int targetHand)
    {
        switch (status)
        {
            case Wield.Status.Ready:
                HUDBar[targetHand].color = Color.black;
                break;
            case Wield.Status.Wind:
                HUDBar[targetHand].color = Color.red;
                break;
            case Wield.Status.Cancel:
                HUDBar[targetHand].color = Color.grey;
                break;
            case Wield.Status.Active:
                HUDBar[targetHand].color = Color.green;
                break;
            case Wield.Status.Strike:
                HUDBar[targetHand].color = Color.magenta;
                break;
            case Wield.Status.Recover:
                HUDBar[targetHand].color = Color.cyan;
                break;
        }
    }

    [ClientRpc]
    public void RpcUpdateItemUsageHUD(float usagePCT, bool resourceCheck)
    {
        if (!isLocalPlayer)
            return;

        HUDItemBackground.alpha = 1;
        HUDItemBar.fillAmount = Mathf.Lerp(HUDItemBar.fillAmount, usagePCT, 36 * Time.deltaTime);
        if (resourceCheck)
            HUDItemBar.color = Color.green;
        else
            HUDItemBar.color = Color.red;
    }

    [Server]
    public void IsUsingItem(bool to, bool soft)
    {
        isUsingItem = to;

        if (!to)
        {
            RpcFinishItem(soft);
        }
    }

    [ClientRpc]
    void RpcFinishItem(bool soft)
    {
        if (!isLocalPlayer)
            return;

        if (!soft)
        {
            HUDItemBackground.alpha = 0;
        }
        HUDItemBar.fillAmount = 1;
    }
    #endregion

    #region Animator
    [ClientRpc]
    void RpcControlAnimator(int targetHand, Pose newPose, Defs.WeaponType weapon)
    {
        //Pose
        pose = newPose;

        //Reset All Bools
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(parameter.name, false);
        }

        //Set Target Bool
        string hand;
        if (targetHand == 0)
            hand = "Right";
        else
            hand = "Left";

        anim.SetBool(hand + newPose.ToString() + weapon.ToString(), true);
    }

    [ClientRpc]
    void RpcDirectAnimator(Pose newPose)
    {
        //Pose
        pose = newPose;

        //Reset All Bools
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(parameter.name, false);
        }

        //Set Target Bool
        anim.SetBool(newPose.ToString(), true);
    }
    #endregion
}