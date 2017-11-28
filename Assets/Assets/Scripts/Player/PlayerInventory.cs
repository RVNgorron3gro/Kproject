using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInventory : NetworkBehaviour
{
    public List<InventoryItem> inventory;
    public LayerMask itemLayerMask;
    public PlayerCore playerCore;

    [System.Serializable]
    public class InventoryItem
    {
        public Item item;
        public int quantity;
    }

    [System.Serializable]
    public struct InventoryItemClient
    {
        public int ID;
        public int quantity;
    }

    [System.Serializable]
    public struct ProximityContainer
    {
        public NetworkInstanceId netHash;
        public string header;
        public InventoryItemClient[] contained;
    }

    void Start()
    {
        playerCore = GetComponent<PlayerCore>();

        if (isServer)
        {
            AddItem(3, 20);
            AddItem(4, 4);
        }
    }

    //ONLY 2 WEAPONS
    //As many other items as you want, but only 1 stack

    [Server]
    public void AddItem(int ID, int amount)
    {
        bool found = false;
        for (int count = 0; count < inventory.Count; count++)
        {
            if (inventory[count].item.ID == ID)
            {
                inventory[count].quantity += amount;
                found = true;
            }
        }

        if (!found)
        {
            InventoryItem newEntry = new InventoryItem()
            {
                item = MasterListDatabase.i.FetchItem(ID),
                quantity = amount
            };
            inventory.Add(newEntry);
        }

        //Update HotBar
        playerCore.items = GetConsumeables();
        PreUpdate(ProcessInventoryItem(playerCore.items));
    }

    [Server]
    public void RemoveItem(int ID, int amount)
    {
        for (int count = 0; count < inventory.Count; count++)
        {
            if (inventory[count].item.ID == ID)
            {
                inventory[count].quantity -= amount;
                if (inventory[count].quantity <= 0)
                {
                    inventory.RemoveAt(count);
                }
                break;
            }
        }

        //Update HotBar
        playerCore.items = GetConsumeables();
        PreUpdate(ProcessInventoryItem(playerCore.items));
    }

    [Server]
    public List<InventoryItem> GetConsumeables()
    {
        List<InventoryItem> fetched = new List<InventoryItem>();
        for (int count = 0; count < inventory.Count; count++)
        {
            if (inventory[count].item.Category == Defs.ItemCategory.Consumeable)
            {
                fetched.Add(inventory[count]);
            }
        }
        return fetched;
    }

    [Command]
    public void CmdFindItemsInProximity()
    {
        Collider[] proximityItems = Physics.OverlapSphere(transform.position, 1.5f, itemLayerMask);
        ProximityContainer[] proximityItemsList = new ProximityContainer[proximityItems.Length];
        for (int cols = 0; cols < proximityItems.Length; cols++)
        {
            Chest.ChestTransfer target = proximityItems[cols].GetComponent<Chest>().contained;

            InventoryItemClient[] inventoryItems = new InventoryItemClient[target.contained.Count];
            for (int count = 0; count < target.contained.Count; count++)
            {
                InventoryItemClient converted = new InventoryItemClient
                {
                    ID = target.contained[count].itemID,
                    quantity = target.contained[count].quantity
                };
                inventoryItems[count] = converted;
            }

            ProximityContainer proximityList = new ProximityContainer
            {
                contained = inventoryItems,
                header = target.id,
                netHash = target.netHash,
            };
            proximityItemsList[cols] = proximityList;
        }
        RpcReturnProximityItems(proximityItemsList);
    }

    [ClientRpc]
    void RpcReturnProximityItems(ProximityContainer[] container)
    {
        if (isLocalPlayer)
        {
            UI_CharacterMenu.i.MenuDisplayProximity(container);
        }
    }

    [Command]
    public void CmdRequestProximityItem(int index, int slot, int id, ProximityContainer[] savedProximity)
    {
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        for (int count = 0; count < chests.Length; count++)
        {
            if (chests[count].GetComponent<NetworkIdentity>().netId == savedProximity[index].netHash)
            {
                chests[count].GetComponent<Chest>().RequestItem(gameObject, slot, id);
                break;
            }
        }
    }

    [Command]
    public void CmdRequestInventory()
    {
        InventoryItemClient[] fetch = new InventoryItemClient[inventory.Count];
        for(int count = 0; count < inventory.Count; count++)
        {
            fetch[count].ID = inventory[count].item.ID;
            fetch[count].quantity = inventory[count].quantity;
        }
        RpcCallbackInventory(fetch);
    }

    [ClientRpc]
    public void RpcCallbackInventory(InventoryItemClient[] callback)
    {
        if (isLocalPlayer)
        {
            UI_CharacterMenu.i.MenuInventoryCallback(callback);
        }
    }

    public KeyValuePair<int, int>[] ProcessInventoryItem(List<InventoryItem> target)
    {
        int[] id = new int[target.Count];
        int[] quantity = new int[target.Count];
        KeyValuePair<int, int>[] val = new KeyValuePair<int, int>[target.Count];
        for (int count = 0; count < target.Count; count++)
        {
            id[count] = target[count].item.ID;
            quantity[count] = target[count].quantity;
            val[count] = new KeyValuePair<int, int>(id[count], quantity[count]);
        }
        return val;
    }

    [Server]
    void PreUpdate(KeyValuePair<int, int>[] vals)
    {
        int[] id = new int[vals.Length];
        int[] quantity = new int[vals.Length];
        for (int count = 0; count < vals.Length; count++)
        {
            id[count] = vals[count].Key;
            quantity[count] = vals[count].Value;
        }
        RpcUpdateHUD(id, quantity);
    }

    [ClientRpc]
    public void RpcUpdateHUD(int[] id, int[] quantity)
    {
        if (isLocalPlayer)
        {
            UI_HUD.i.UpdateHotBar(id, quantity);
        }
    }
}