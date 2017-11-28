using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionBarSlots : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        int ability = eventData.pointerDrag.GetComponent<AbilityUpgradeSlot>().ability;
        UI_HUD.i.playerCore.CmdAssignAbility(ability, transform.GetSiblingIndex());
    }
}