using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform menuRoot;
    CanvasGroup canvas;
    UI_Map menu;

    [Header("Interaction")]
    public static GameObject selected;
    public Transform parent;

    public TroopCore unit;

    void Start()
    {
        menuRoot = GameObject.Find("HUD").transform;
        canvas = GetComponent<CanvasGroup>();
        menu = menuRoot.GetChild(1).GetComponent<UI_Map>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        selected = gameObject;
        parent = transform.parent;
        transform.SetParent(menuRoot);

        //Canvas Group
        canvas.blocksRaycasts = false;
        canvas.interactable = false;

        //Mouse
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //HUD
        //Debug.Log(Helper.IsBarracksUnit(Helper.ConvertUnitTypeToInt(unit.type)));
        if (!Helper.IsBarracksUnit(Helper.ConvertUnitTypeToInt(unit.type)))
        {
            menu.Centre_HighlightMap(UI_Map.HighlightCritera.BuildableRegions);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        selected = null;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        //Canvas Group
        canvas.blocksRaycasts = true;
        canvas.interactable = true;

        //Mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        menu.Centre_InitialiseMap(UI_Map.HighLightMode.Darken);
        menu.Centre_HighlightMap(menu.right_BuildingSelected.masterRegion, UI_Map.HighLightMode.Lighten);
    }
}
