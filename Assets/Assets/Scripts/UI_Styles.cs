using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Styles : MonoBehaviour
{
    public static UI_Styles i;

    [System.Serializable] public class Resources
    {
        public Color hostility;
        public Color health;
        public Color stamina;
    }
    public Resources resources = new Resources();

    [System.Serializable] public class UnitHUD
    {
        public Vector3 offset;
        public GameObject barLarge;
        public GameObject barMedium;
        public GameObject barSmall;
        public GameObject containerLarge;
        public GameObject containerMedium;
        public GameObject containerSmall;
        public GameObject emptyHolder;
        public GameObject popup;
        public GameObject guideLine;
        public float guideLineSpacing;
        public GameObject ability;
        public Abilities abilitySheet;
        public UnitHUDStyles defaultStyle;
    }
    public UnitHUD unitHUD = new UnitHUD();

    void Awake()
    {
        if (!i)
        {
            i = this;
        }
        else
        {
            Debug.LogError(this + " : THERE ARE MULTIPLE INSTANCES OF THIS SCRIPT");
            Destroy(this);
        }

        //Protection
        if(unitHUD.guideLineSpacing <= 0)
        {
            unitHUD.guideLine = null;
            Debug.LogError("Guide Line Spacing is 0!");
        }
    }
}