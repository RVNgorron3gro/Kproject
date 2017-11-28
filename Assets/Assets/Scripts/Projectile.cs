using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    public WeaponCore caller;
    public int itemID;

    public Effector effector;
    public int targetLimit;
    public int targetsHit;
    public List<NetworkInstanceId> hitTargets = new List<NetworkInstanceId>();
    public Quaternion instantiateRotation;
    public float lifespan;
    [SyncVar]
    public float currentTime;

    [ServerCallback]
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= lifespan)
        {
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }
    }

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Terrain")
        {
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }

        if (other.GetComponent<NetworkIdentity>())
        {
            if (!hitTargets.Contains(other.GetComponent<NetworkIdentity>().netId))
            {
                hitTargets.Add(other.GetComponent<NetworkIdentity>().netId);
                if (other.tag == "Critter")
                {
                    RegisterHit(other.gameObject);
                    ///caller.RpcNotifyHit();
                }
                else if (other.tag == "Troop")
                {

                }
                else if (other.tag == "Player")
                {
                    RegisterHit(other.gameObject);
                    ///caller.RpcNotifyHit();
                }
            }
        }
    }

    [Server]
    public void RegisterHit(GameObject hit)
    {
        //Audio
        AudioController.i.CmdPlay3DItemSound(caller.playerCore.currentLocation, itemID, Defs.ItemSound.HitSound, transform.position, 0.8f, 10, 30, 0.85f, 1.15f);

        UnitCore target = hit.GetComponent<UnitCore>();
        target.RunEffector(new Source(caller.unitCore, MasterListDatabase.i.FetchItem(itemID)), effector, false);
        if (targetLimit != 0)
        {
            targetsHit--;
            if (targetsHit == 0)
                Destroy(gameObject);
        }
    }

    [Server]
    public void RegisterBlock(GameObject hit)
    {
        if (!hitTargets.Contains(hit.GetComponent<NetworkIdentity>().netId))
        {
            hitTargets.Add(hit.GetComponent<NetworkIdentity>().netId);
            Debug.Log("Blocked!");
            UnitCore target = hit.GetComponent<UnitCore>();
            target.RunEffector(new Source(caller.unitCore, MasterListDatabase.i.FetchItem(itemID), caller), effector, true);
            Destroy(gameObject);
        }
    }
}