using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityUpgradeSlot : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Transform root;
    CanvasGroup canvas;

    public Vector3 originalPos;
    public TextMeshProUGUI descriptionText;
    public Ability targetAbility;
    public int ability;
    public string description;
    public bool canUse;

    void Start()
    {
        root = transform.parent;
        canvas = GetComponent<CanvasGroup>();

        ability = transform.parent.GetSiblingIndex();
        descriptionText = transform.parent.parent.parent.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        targetAbility = UI_Styles.i.unitHUD.abilitySheet.contained[transform.parent.GetSiblingIndex()];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string description;
        if (canUse)
        {
            description = "<size=32><color=orange>" + targetAbility.title + "</color></size> \n" + targetAbility.description;
        }
        else
        {
            description = "<size=32><color=#787878>" + targetAbility.title + "</color></size> \n" + targetAbility.description;
        }
        descriptionText.text = description;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canUse)
            return;

        transform.SetParent(transform.parent.parent.parent);

        //Canvas Group
        canvas.blocksRaycasts = false;
        canvas.interactable = false;

        //Mouse
        Cursor.visible = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canUse)
            return;
        transform.position = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.SetParent(root);
        transform.localPosition = Vector3.zero;

        //Canvas Group
        canvas.blocksRaycasts = true;
        canvas.interactable = true;

        //Mouse
        Cursor.visible = true;
    }
}