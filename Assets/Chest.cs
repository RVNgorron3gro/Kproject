using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Chest : MonoBehaviour
{
    [System.Serializable]
    public class ChestTransfer
    {
        public NetworkInstanceId netHash;
        public string id;
        public List<ChestItems> contained = new List<ChestItems>();
    }

    [System.Serializable]
    public class ChestItems
    {
        public int itemID;
        public int quantity;
    }
    public ChestTransfer contained;

    void Start()
    {
        contained.netHash = GetComponent<NetworkIdentity>().netId;
    }

    public void RequestItem(GameObject player, int slot, int id)
    {
        if (contained.contained[slot].itemID == id)
        {
            player.GetComponent<PlayerInventory>().AddItem(contained.contained[slot].itemID, contained.contained[slot].quantity);
            contained.contained.RemoveAt(slot);
            player.GetComponent<PlayerInventory>().CmdFindItemsInProximity();
        }
    }
}