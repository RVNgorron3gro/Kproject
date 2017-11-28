using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class PopupDirector : NetworkBehaviour
{
    /*
    [HideInInspector] public static PopupDirector i;

    [Header("References")]
    public Camera cam;
    public GameObject popupObject;
    public Transform popupParent;
    public List<GameObject> popups;
    public float popupSizeMin, popupSizeMax;

    void Awake()
    {
        if (!i)
        {
            Debug.Log("Made!");
            i = this;
        }
        else
        {
            Debug.LogError(this + " : THERE ARE MULTIPLE INSTANCES OF THIS SCRIPT");
            Destroy(this);
        }
    }

    void Start()
    {
        cam = Camera.main;
        popupParent = GameObject.Find("HUD/Overlay/Popups").transform;

        //Get Popup Sizes
        popupSizeMin = UI_Styles.i.popup.popupSizeMin;
        popupSizeMax = UI_Styles.i.popup.popupSizeMax;
    }

    public void CreatePopup(float amount, float percentage, Quaternion hitRotation, Transform target)
    {
        percentage = Mathf.Abs(percentage);
        GameObject newPopup = Instantiate(popupObject, popupParent, false);
        popups.Add(newPopup);
        Quaternion converted = Quaternion.Euler(0, hitRotation.eulerAngles.y, 0);
        Quaternion newRot = Quaternion.Euler(converted.eulerAngles + new Vector3(0, Random.Range(-35, 35), 0));
        newPopup.transform.rotation = newRot;
        newPopup.name = "Popup @" + Time.time + " (OF " + amount + ")";
        TextMeshProUGUI newText = newPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        newText.transform.rotation = Quaternion.Euler(0, 0, 0);

        newText.text = Mathf.Abs(amount).ToString("F0");
        Color newColor = Color.white;
        if (amount > 0)
        {
            newColor = Color.green;
        }
        else
        {
            newColor = Color.red;
        }
        newText.color = newColor;
        newText.fontSize = Mathf.Clamp(((popupSizeMax - popupSizeMin) * percentage) + popupSizeMin, popupSizeMin, popupSizeMax);

        PopupController targetPopup = newPopup.GetComponent<PopupController>();
        targetPopup.target = target;
    }
    */
}