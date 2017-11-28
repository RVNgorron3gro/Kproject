using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class OldWeaponCore : NetworkBehaviour
{
    /*
    [Header("Audio")]
    public AudioClip clipHit;

    [Header("References")]
    Animator animator;
    UnitCore unitCore;
    public PlayerCore playerCore;
    public LayerMask projectileMask;
    [HideInInspector]
    public float activeBlockAngle;

    [Header("Wielding")]
    public List<Wield> hand = new List<Wield>();

    public GameObject weaponObj;
    [SyncVar]
    public bool recourseCheck;

    [System.Serializable]
    public class Wield
    {
        [Header("Held Weapon")]
        public Item equipped;
        public Melee meleeController;

        [Header("Mechanics")]
        public bool cancel;
        public bool recover;
        public float currentWindup;
        public float currentRecoveryTime;

        [Header("Striking")]
        public List<int> hitTargets = new List<int>();
    }
    public int activeHand;
    public float currentStrikeTime;

    [Header("Animation")]
    [SyncVar]
    public Pose currentPose;

    [Header("Shooting")]
    public Transform rightHandSpawner;

    [Header("UI")]
    public Image windupPointer;
    public Image windupBack;
    public List<Image> windup = new List<Image>();

    void Start()
    {
        //Get References
        unitCore = GetComponent<UnitCore>();
        playerCore = GetComponent<PlayerCore>();
        animator = GetComponent<Animator>();

        //make weapon (TEMPPP!!!!!!)
        for (int count = 0; count < 2; count++)
        {
            GameObject obj;
            if (count == 0)
            {
                obj = Instantiate(weaponObj, transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0), false);
            }
            else
            {
                obj = Instantiate(weaponObj, transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0), false);
            }
            Physics.IgnoreCollision(obj.GetComponent<Collider>(), GetComponent<Collider>());

            if (isServer)
                NetworkServer.Spawn(obj);

            hand.Add(new Wield());

            hand[count].equipped = MasterListDatabase.i.FetchItem(count + 1);
            hand[count].meleeController = obj.GetComponent<Melee>();
            hand[count].meleeController.controller = this;
            hand[count].meleeController.handID = count;
        }

        //Transforms
        rightHandSpawner = transform.GetChild(3);

        //HUD
        windupPointer = GameObject.Find("WindupPointer").GetComponent<Image>();
        windupBack = GameObject.Find("WindupPointer/WindupBack").GetComponent<Image>();
        for (int count = 0; count < 2; count++)
        {
            windup.Add(windupBack.transform.GetChild(count).GetComponent<Image>());
        }


        if (isClient)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    [Command]
    void CmdFindProjectilesInShieldArea(int targetHand)
    {
        unitCore.isBlocking = true;
        int shieldAngle = hand[targetHand].equipped.BlockAngle;
        activeBlockAngle = shieldAngle;

        Collider[] projectilesInRadius = Physics.OverlapSphere(transform.position, 1.25f, projectileMask);
        for (int count = 0; count < projectilesInRadius.Length; count++)
        {
            Transform target = projectilesInRadius[count].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < shieldAngle / 2)
            {
                Debug.Log("Found projectile!");
                target.GetComponent<Projectile>().RegisterBlock(gameObject);
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        ClientUpdate();
    }

    [Command]
    void CmdCheckRecourse(int count)
    {
        recourseCheck = EffectorMethods.CheckIfEnoughResources(unitCore, hand[count].equipped.CostEffector);
    }

    [ClientCallback]
    void ClientUpdate()
    {
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            activeHand = -1;
            CmdControlAnimator(Pose.Idle);
        }

        bool isCanceled = false;
        for (int count = 0; count < 2; count++)
        {
            if (hand[count].cancel)
            {
                isCanceled = true;
            }
        }

        for (int count = 0; count < 2; count++)
        {
            if (hand[count].equipped != null)
            {
                CmdCheckRecourse(count);
                if (Input.GetMouseButton(count) && !isCanceled && !hand[count].recover && currentStrikeTime == 0 && (activeHand == count || activeHand == -1) && recourseCheck && !UI_Chat.i.active)
                {
                    Windup(count);
                }
                else
                {
                    if (hand[count].currentWindup != 0)
                    {
                        if (hand[count].equipped.WeaponType == Defs.WeaponType.Projectile)
                        {
                            unitCore.CmdIsWinding(false, false, false);
                        }

                        if (hand[count].equipped.WeaponType == Defs.WeaponType.Shield)
                        {
                            unitCore.isBlocking = false;
                            hand[count].currentWindup = 0;
                            hand[count].currentRecoveryTime = 0.1f;
                            currentStrikeTime = 0;
                            hand[count].recover = true;
                            activeHand = -1;
                        }

                        hand[count].cancel = true;
                    }
                    else
                    {
                        hand[count].cancel = false;

                        if (hand[count].currentRecoveryTime == 0)
                        {
                            hand[count].recover = false;
                        }
                    }
                }

                windupBack.color = Color.black;
            }
            else
            {
                windup[count].color = Color.clear;
                windupBack.color = Color.clear;
            }
            DecayWindup(count);
        }

        //UI
        windupPointer.rectTransform.position = Input.mousePosition;
        windupPointer.color = Color.Lerp(windupPointer.color, Color.white, 0.1f);
    }

    void Windup(int targetHand)
    {
        if (hand[targetHand].equipped.WeaponType == Defs.WeaponType.Projectile)
        {
            unitCore.CmdIsWinding(true, true, false);
        }

        activeHand = targetHand;
        hand[targetHand].currentWindup = Mathf.Clamp(hand[targetHand].currentWindup + Time.deltaTime, 0, hand[targetHand].equipped.WindupTime);
        UpdateHUD(UpdateMode.WindupTime);
        if (hand[targetHand].currentWindup != hand[targetHand].equipped.WindupTime)
        {
            if (targetHand == 0)
            {
                CmdControlAnimator(Pose.PreStrikeRight);
            }
            else
            {
                CmdControlAnimator(Pose.PreStrikeLeft);
            }
        }
        else
        {
            int ready = 0;
            for (int count = 0; count < 2; count++)
            {
                if (!hand[count].cancel)
                    ready++;
            }

            if (ready == 2)
                ClientRelease(unitCore.id, targetHand);
        }
    }

    void DecayWindup(int targetHand)
    {
        if (currentStrikeTime == 0)
        {
            if (hand[targetHand].recover)
            {
                hand[targetHand].currentRecoveryTime = Mathf.Clamp(hand[targetHand].currentRecoveryTime - Time.deltaTime, 0, hand[targetHand].equipped.RecoveryTime);
                UpdateHUD(UpdateMode.RecoveryTime);
                if (hand[targetHand].equipped.WeaponType == Defs.WeaponType.Melee)
                {
                    unitCore.CmdIsWinding(false, false, true);
                }
            }
            else
            {
                if (hand[targetHand].equipped.WeaponType == Defs.WeaponType.Melee)
                {
                    unitCore.CmdIsWinding(false, false, false);
                }
            }

            if (hand[targetHand].cancel)
            {
                hand[targetHand].currentWindup = Mathf.Clamp(hand[targetHand].currentWindup - Time.deltaTime, 0, hand[targetHand].equipped.WindupTime);
                UpdateHUD(UpdateMode.WindupCancel);
            }
        }
        else
        {
            currentStrikeTime = Mathf.Clamp(currentStrikeTime - Time.deltaTime, 0, hand[targetHand].equipped.StrikeTime);
            UpdateHUD(UpdateMode.StrikeTime);
        }
    }

    void ClientRelease(int id, int targetHand)
    {
        if (hand[targetHand].equipped.WeaponType != Defs.WeaponType.Shield)
        {
            hand[targetHand].currentWindup = 0;
            hand[targetHand].currentRecoveryTime = hand[targetHand].equipped.RecoveryTime;
            currentStrikeTime = hand[targetHand].equipped.StrikeTime;
            hand[targetHand].recover = true;
            activeHand = -1;
            CmdRelease(id, targetHand);
        }
        else
        {
            CmdFindProjectilesInShieldArea(targetHand);
        }
    }

    [Command]
    void CmdRelease(int id, int targetHand)
    {
        AudioController.i.CmdPlay3DItemSound(playerCore.currentLocation, hand[targetHand].equipped.ID, Defs.ItemSound.ReleaseSound, transform.position, 0.8f, 10, 30, 0.85f, 1.15f);
        hand[targetHand].hitTargets.Clear();
        switch (hand[targetHand].equipped.WeaponType)
        {
            case Defs.WeaponType.Melee:
                hand[targetHand].meleeController.callerID = id;
                hand[targetHand].meleeController.effector = hand[targetHand].equipped.TargetEffector;
                hand[targetHand].meleeController.instantiateRotation = unitCore.transform.rotation;
                hand[targetHand].meleeController.hitTargets.Clear();
                CmdControlAnimator(Pose.Strike);
                break;
            case Defs.WeaponType.Projectile:
                CmdControlAnimator(Pose.Strike);
                GameObject go = Instantiate(hand[targetHand].equipped.ProjectileObject, new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), Quaternion.Euler(-90, transform.rotation.eulerAngles.y + 180, 0));
                Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());
                go.GetComponent<Rigidbody>().AddForce(rightHandSpawner.forward * hand[targetHand].equipped.Speed, ForceMode.VelocityChange);
                Projectile controller = go.GetComponent<Projectile>();
                //controller.callerID = id;
                controller.caller = this;
                controller.effector = hand[targetHand].equipped.TargetEffector;
                controller.targetLimit = controller.targetsHit = hand[targetHand].equipped.TargetLimit;
                controller.instantiateRotation = unitCore.transform.rotation;
                controller.lifespan = hand[targetHand].equipped.Range / hand[targetHand].equipped.Speed;
                NetworkServer.Spawn(go);
                break;
        }

        //Now Take Costs
        unitCore.RunEffector(hand[targetHand].equipped.CostEffector, false);
    }

    [Server]
    public void SpawnProjectile(Item toShoot)
    {
        GameObject go = Instantiate(toShoot.ProjectileObject, new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z), Quaternion.Euler(-90, transform.rotation.eulerAngles.y + 180, 0));
        Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());
        go.GetComponent<Rigidbody>().AddForce(rightHandSpawner.forward * toShoot.Speed, ForceMode.VelocityChange);
        Projectile controller = go.GetComponent<Projectile>();
        //controller.callerID = unitCore.id;
        controller.caller = this;
        controller.effector = toShoot.TargetEffector;
        controller.targetLimit = controller.targetsHit = toShoot.TargetLimit;
        controller.instantiateRotation = unitCore.transform.rotation;
        controller.lifespan = toShoot.Range / toShoot.Speed;
        NetworkServer.Spawn(go);
    }

    public enum UpdateMode
    {
        WindupTime, RecoveryTime, WindupCancel, StrikeTime
    }

    void UpdateHUD(UpdateMode mode)
    {
        if (isLocalPlayer)
        {
            for (int count = 0; count < 2; count++)
            {
                switch (mode)
                {
                    case UpdateMode.WindupTime:
                        windup[count].fillAmount = hand[count].currentWindup / hand[count].equipped.WindupTime;
                        windup[count].color = Color.red;
                        break;
                    case UpdateMode.RecoveryTime:
                        windup[count].fillAmount = hand[count].currentRecoveryTime / hand[count].equipped.RecoveryTime;
                        windup[count].color = Color.green;
                        break;
                    case UpdateMode.WindupCancel:
                        windup[count].fillAmount = hand[count].currentWindup / hand[count].equipped.WindupTime;
                        windup[count].color = Color.grey;
                        break;
                    case UpdateMode.StrikeTime:
                        windup[count].fillAmount = currentStrikeTime / hand[count].equipped.StrikeTime;
                        windup[count].color = Color.cyan;
                        break;
                }
            }
        }
    }

    public enum Pose
    {
        Idle, PreStrikeRight, PreStrikeLeft, Strike
    }

    [Command]
    void CmdControlAnimator(Pose newPose)
    {
        RpcSetPose(newPose);
        switch (newPose)
        {
            case Pose.Idle:
                currentPose = Pose.Idle;
                break;
            case Pose.PreStrikeRight:
                currentPose = Pose.PreStrikeRight;
                break;
            case Pose.PreStrikeLeft:
                currentPose = Pose.PreStrikeLeft;
                break;
            case Pose.Strike:
                currentPose = Pose.Strike;
                break;
        }
    }

    [ClientRpc]
    void RpcSetPose(Pose newPose)
    {
        animator.SetBool("Idle", false); animator.SetBool("PreStrikeRight", false); animator.SetBool("PreStrikeLeft", false); animator.SetBool("Strike", false);

        switch (newPose)
        {
            case Pose.Idle:
                animator.SetBool("Idle", true);
                break;
            case Pose.PreStrikeRight:
                animator.SetBool("PreStrikeRight", true);
                break;
            case Pose.PreStrikeLeft:
                animator.SetBool("PreStrikeLeft", true);
                break;
            case Pose.Strike:
                animator.SetBool("Strike", true);
                break;
        }
    }

    [ClientRpc]
    public void RpcNotifyHit()
    {
        if (isLocalPlayer)
        {
            windupPointer.color = Color.red;
            AudioController.i.Play2DSound(clipHit, 1, 0.9f, 1.1f);
        }
    }
    */
}