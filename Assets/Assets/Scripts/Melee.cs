using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Melee : MonoBehaviour
{
    public WeaponCore caller;
    public int itemID;
    public int hand;
    public bool active;

    public Effector effector;
    public List<NetworkInstanceId> hitTargets = new List<NetworkInstanceId>();

    private void OnTriggerStay(Collider other)
    {
        if (active)
        {
            if (other.GetComponent<NetworkIdentity>())
            {
                if (!hitTargets.Contains(other.GetComponent<NetworkIdentity>().netId))
                {
                    hitTargets.Add(other.GetComponent<NetworkIdentity>().netId);
                    if (other.tag == "Critter")
                    {
                        RegisterHit(other.gameObject);
                        ///controller.RpcNotifyHit();
                    }
                    else if (other.tag == "Troop")
                    {

                    }
                    else if (other.tag == "Player")
                    {
                        int blockAngle = other.GetComponent<WeaponCore>().activeBlockAngle;
                        if (blockAngle != 0)
                        {
                            Transform target = other.transform;
                            Vector3 dirToTarget = (caller.transform.position - target.position).normalized;
                            Debug.Log(transform.forward);
                            Debug.Log(dirToTarget);
                            Debug.Log(blockAngle);
                            Debug.Log(Vector3.Angle(target.forward, dirToTarget));
                            Debug.Log(Vector3.Angle(target.forward, dirToTarget) < blockAngle);
                            if (Vector3.Angle(target.forward, dirToTarget) < blockAngle)
                            {
                                RegisterBlock(other.gameObject);
                            }
                            else
                            {
                                RegisterHit(other.gameObject);
                            }
                        }
                        else
                        {
                            RegisterHit(other.gameObject);
                        }
                        ///controller.RpcNotifyHit();
                    }
                }
            }
        }
    }

    void RegisterHit(GameObject hit)
    {
        //Audio
        AudioController.i.CmdPlay3DItemSound(caller.playerCore.currentLocation, caller.hands[hand].equipped.ID, Defs.ItemSound.HitSound, transform.position, 0.8f, 10, 30, 0.85f, 1.15f);

        UnitCore target = hit.GetComponent<UnitCore>();
        target.RunEffector(new Source(caller.unitCore, MasterListDatabase.i.FetchItem(itemID), caller), effector, false);
    }

    public void RegisterBlock(GameObject hit)
    {
        Debug.Log("Blocked!");
        UnitCore target = hit.GetComponent<UnitCore>();
        target.RunEffector(new Source(caller.unitCore, MasterListDatabase.i.FetchItem(itemID), caller), effector, true);
    }
}