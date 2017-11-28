using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UnitHUD : MonoBehaviour
{
    Camera cam;
    Canvas canvas;
    CanvasGroup canvasGroup;
    CanvasGroup visionCanvasGroup;
    UnitCore parent;
    float timeout;

    [Header("Owner")]
    public Transform ownerT;
    public Vector3 offset;
    public Vector3 screenOffset;

    [System.Serializable]
    public class ContainedElements
    {
        public Transform root;
        public Defs.ResourceTypes resource;
        public Image fill;
        public Image reductionFill;
        public bool reductionAnimation;
        public float lastReportedVal;
        public float sinceLastLoss;
        public int guideLines;
    }
    public List<ContainedElements> elements = new List<ContainedElements>();

    [Header("Build")]
    public GameObject guideLine;

    [Header("Abilities")]
    public Image ability;
    public Abilities abilitySheet;

    [Header("Instance")]
    public GameObject target;

    public void Setup(UnitCore caller, string[] activeResources, Defs.UnitType mode, bool isHostile)
    {
        parent = caller;
        cam = Camera.main;
        canvas = GameObject.Find("UI").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        ownerT = caller.transform;
        offset = UI_Styles.i.unitHUD.offset;

        //Get the prefabs
        guideLine = UI_Styles.i.unitHUD.guideLine;

        //Create the Container
        GameObject targetContainer;
        switch (Parameters.i.uHContainer[(int)mode])
        {
            case Defs.LayoutSize.Large:
                targetContainer = UI_Styles.i.unitHUD.containerLarge;
                break;
            case Defs.LayoutSize.Medium:
                targetContainer = UI_Styles.i.unitHUD.containerMedium;
                break;
            case Defs.LayoutSize.Small:
                targetContainer = UI_Styles.i.unitHUD.containerSmall;
                break;
            default:
                targetContainer = new GameObject();
                Debug.LogError("Switch Break! @Container");
                break;
        }
        GameObject creation = Instantiate(targetContainer, transform);
        if (isHostile)
            creation.GetComponent<Image>().color = UI_Styles.i.resources.hostility;

        //Create the Bars
        for (int resource = 0; resource < activeResources.Length; resource++)
        {
            Parameters.Resource definiton = Parameters.i.GetResourceDefinition((Defs.ResourceTypes)Enum.Parse(typeof(Defs.ResourceTypes), activeResources[resource]));

            GameObject targetGauge;
            switch (definiton.uHBar[(int)mode])
            {
                case Defs.LayoutSize.Large:
                    targetGauge = UI_Styles.i.unitHUD.barLarge;
                    screenOffset += new Vector3(0, 20);
                    break;
                case Defs.LayoutSize.Medium:
                    targetGauge = UI_Styles.i.unitHUD.barMedium;
                    screenOffset += new Vector3(0, 14);
                    break;
                case Defs.LayoutSize.Small:
                    targetGauge = UI_Styles.i.unitHUD.barSmall;
                    screenOffset += new Vector3(0, 8);
                    break;
                default:
                    targetGauge = new GameObject();
                    Debug.LogError("Switch Break! @Gauge");
                    break;
            }

            GameObject newGaugeObject = Instantiate(targetGauge, transform.GetChild(0));
            ContainedElements newGauge = new ContainedElements
            {
                resource = (Defs.ResourceTypes)Enum.Parse(typeof(Defs.ResourceTypes), definiton.Name),
                root = newGaugeObject.transform,
                reductionAnimation = definiton.reductionAnimation,
                guideLines = definiton.guideLineInterval,
            };
            newGauge.fill = newGauge.root.GetChild(1).GetComponent<Image>();
            if (newGauge.reductionAnimation)
                newGauge.reductionFill = newGauge.root.GetChild(0).GetComponent<Image>();
            elements.Add(newGauge);

            //Set Color
            newGauge.fill.color = definiton.Color;
        }

        if(caller.mode == Defs.UnitType.Player)
        {
            GameObject holder = Instantiate(UI_Styles.i.unitHUD.ability, transform);
            ability = holder.GetComponent<Image>();
            ability.color = Color.clear;
            abilitySheet = UI_Styles.i.unitHUD.abilitySheet;
        }

        //Create the Guide Lines
        //UpdateGuideLines(caller);

        //Now Initialise the HUD
        //RefreshAll(caller);

        visionCanvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        FadeMode(true);
    }

    public void RefreshAll(UnitCore snapshot)
    {
        for (int count = 0; count < elements.Count; count++)
        {
            float newAmount = 0;
            switch (elements[count].resource)
            {
                case Defs.ResourceTypes.Health:
                    newAmount = snapshot.Health.PCT();
                    break;
                case Defs.ResourceTypes.Stamina:
                    newAmount = snapshot.Stamina.PCT();
                    break;
            }
            elements[count].fill.fillAmount = newAmount;

            if (elements[count].reductionAnimation)
            {
                if (elements[count].lastReportedVal > newAmount)
                {
                    elements[count].sinceLastLoss = 1.2f;
                }
                else
                {
                    elements[count].sinceLastLoss = 0;
                }
            }

            elements[count].lastReportedVal = newAmount;
        }
    }

    public void Refresh(Defs.ResourceTypes targetResource, Transfer pack)
    {
        for (int count = 0; count < elements.Count; count++)
        {
            if (elements[count].resource == targetResource)
            {
                elements[count].fill.fillAmount = pack.PCT;

                if (elements[count].reductionAnimation)
                {
                    if (elements[count].lastReportedVal > pack.PCT)
                    {
                        elements[count].sinceLastLoss = 1.2f;
                    }
                    else
                    {
                        elements[count].sinceLastLoss = 0;
                    }
                }

                elements[count].lastReportedVal = pack.PCT;
                break;
            }
        }
    }

    public void UpdateGuideLines(UnitCore target)
    {
        for (int count = 0; count < elements.Count; count++)
        {
            if (elements[count].guideLines >= 100)
            {
                int lines = 0;
                float spacing = 0;
                switch (elements[count].resource)
                {
                    case Defs.ResourceTypes.Health:
                        lines = Mathf.FloorToInt(target.Health.Max / UI_Styles.i.unitHUD.guideLineSpacing);
                        break;
                    case Defs.ResourceTypes.Stamina:
                        lines = Mathf.FloorToInt(target.Stamina.Max / UI_Styles.i.unitHUD.guideLineSpacing);
                        break;
                }
                spacing = ((elements[count].fill.preferredWidth / 2) / lines);
                Transform guideLinesT = elements[count].root.GetChild(2);
                HorizontalLayoutGroup targetGroup = guideLinesT.GetComponent<HorizontalLayoutGroup>();
                targetGroup.spacing = spacing;
                for (int instance = 0; instance < lines; instance++)
                {
                    Instantiate(guideLine, guideLinesT);
                }
            }
        }
    }

    public void FadeMode(bool fade)
    {
        if (fade)
        {
            StartCoroutine(Fade(0.3f));
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    public IEnumerator Fade(float fadeVal)
    {
        float elapsedTime = 0;
        float startVal = canvasGroup.alpha;
        while (elapsedTime < 0.2f)
        {
            canvasGroup.alpha = Mathf.Lerp(startVal, fadeVal, (elapsedTime / 0.2f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ChangeVisiblity(bool to)
    {
        if (to)
        {
            visionCanvasGroup.alpha = 1;
        }
        else
        {
            visionCanvasGroup.alpha = 0;
        }
    }

    public IEnumerator PingAbility(int targetAbility)
    {
        ability.sprite = abilitySheet.contained[targetAbility].image;
        ability.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        ability.color = Color.clear;

    }

    void LateUpdate()
    {
        if (ownerT)
        {
            transform.position = cam.WorldToScreenPoint(ownerT.position + offset) + screenOffset;

            for (int count = 0; count < elements.Count; count++)
            {
                if (elements[count].reductionAnimation)
                {
                    elements[count].sinceLastLoss = Mathf.Clamp(elements[count].sinceLastLoss - Time.deltaTime, 0, 1.2f);
                    float val = elements[count].reductionFill.fillAmount;
                    if (val > elements[count].lastReportedVal)
                    {
                        if (elements[count].sinceLastLoss == 0)
                        {
                            elements[count].reductionFill.fillAmount = Mathf.Lerp(val, elements[count].lastReportedVal, 5 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        elements[count].reductionFill.fillAmount = elements[count].lastReportedVal;
                    }
                }
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }
}