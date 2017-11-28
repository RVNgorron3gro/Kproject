using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSlot : MonoBehaviour, IDropHandler
{
    public Draggable dead;

    public void OnDrop(PointerEventData eventData)
    {
        Draggable drag = Draggable.selected.GetComponent<Draggable>();
        drag.unit.Traverse(name);
    }
}