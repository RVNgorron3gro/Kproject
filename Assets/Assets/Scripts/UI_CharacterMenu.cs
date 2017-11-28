using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UI_CharacterMenu : MonoBehaviour
{
    [HideInInspector]
    public static UI_CharacterMenu i;
    public PlayerCore playerCore;
    public Abilities abilitySheet;
    public Sprite abilityDefault;
    public LayerMask itemLayerMask;
    public PlayerInventory.ProximityContainer[] savedProximity;

    [Header("Root")]
    public CanvasGroup root;

    [Header("Menu")]
    public bool menuActive;
    public float cooldown;
    public CanvasGroup characterMenu;

    [Header("Proximity")]
    public Transform menuProximity;
    public GameObject menuProximityHeader;
    public GameObject menuProximityItem;

    //Characteristics

    [Header("Inventory")]
    public GameObject inventorySlot;
    public Transform menuNC;
    public Transform menuC;

    [Header("Abilities")]
    public Transform menuAbilities;
    public TextMeshProUGUI menuAbilitiesAP;
    public Image menuAbilitiesLevelFill;
    public TextMeshProUGUI menuAbilitiesLevelText;
    public Button menuAbilitiesLearn;

    [Header("Ability")]
    public CanvasGroup abilityMenu;

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
    }

    void Start()
    {
        //Canvas Group
        root = GetComponent<CanvasGroup>();
        characterMenu = transform.GetChild(0).GetComponent<CanvasGroup>();
        abilityMenu = transform.GetChild(1).GetComponent<CanvasGroup>();

        //Menu
        menuAbilities = transform.GetChild(0).GetChild(2).GetChild(5);
        menuAbilitiesLevelFill = menuAbilities.GetChild(1).GetChild(0).GetComponent<Image>();
        menuAbilitiesLevelText = menuAbilities.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        menuAbilitiesAP = menuAbilities.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        menuAbilitiesLearn = menuAbilities.GetChild(2).GetChild(1).GetComponent<Button>();

        //Proximity
        menuProximity = transform.GetChild(0).GetChild(0).GetChild(1);

        //Inventory
        menuNC = transform.GetChild(0).GetChild(2).GetChild(1);
        menuC = transform.GetChild(0).GetChild(2).GetChild(3);

        //Ability

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && cooldown == 0)
        {
            menuActive = !menuActive;
            if (menuActive)
            {
                //HUD
                UI_HUD.i.GetComponent<CanvasGroup>().alpha = 0;

                //Update Elements
                MenuRequestAbilitiesInfo();
                playerCore.GetComponent<PlayerInventory>().CmdFindItemsInProximity();
                MenuInventoryRequest();

                //Root
                StartCoroutine(Fade(1));
                root.blocksRaycasts = true;
                root.interactable = true;

                //UI
                UI_State.ChangeState(true);
            }
            else
            {
                //HUD
                UI_HUD.i.GetComponent<CanvasGroup>().alpha = 1;

                //Root
                root.alpha = 0;
                root.blocksRaycasts = false;
                root.interactable = false;

                //Character
                characterMenu.alpha = 0;
                characterMenu.blocksRaycasts = false;
                characterMenu.interactable = false;

                //Ability
                abilityMenu.alpha = 0;
                abilityMenu.blocksRaycasts = false;
                abilityMenu.interactable = false;

                //UI
                UI_State.ChangeState(false);
            }

            //Apply cooldown
            cooldown = 0.35f;
        }

        if (menuActive)
        {
            //Proximity Update
            Collider[] proximityItems = Physics.OverlapSphere(playerCore.transform.position, 1.5f, itemLayerMask);
            if (savedProximity.Length != proximityItems.Length)
            {
                playerCore.GetComponent<PlayerInventory>().CmdFindItemsInProximity();
            }
        }

        cooldown = Mathf.Clamp(cooldown - Time.deltaTime, 0, 0.6f);
    }

    //MENU PROXIMITY
    public void MenuDisplayProximity(PlayerInventory.ProximityContainer[] items)
    {
        foreach (Transform t in menuProximity)
        {
            Destroy(t.gameObject);
        }

        for (int count = 0; count < items.Length; count++)
        {
            Instantiate(menuProximityHeader, menuProximity, false).GetComponent<TextMeshProUGUI>().text = items[count].header;

            for (int entry = 0; entry < items[count].contained.Length; entry++)
            {
                GameObject target = Instantiate(menuProximityItem, menuProximity, false);
                Item item = MasterListDatabase.i.FetchItem(items[count].contained[entry].ID);
                target.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = item.Image;
                target.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "(" + items[count].contained[entry].quantity + ") " + item.Title;

                ProximityItem pi = target.AddComponent<ProximityItem>();
                pi.index = count;
                pi.slot = entry;
                pi.id = item.ID;
                target.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { MenuRequestProximityItem(pi); });
            }
        }
        savedProximity = items;

        //Update Inventory
        MenuInventoryRequest();
    }

    void MenuRequestProximityItem(ProximityItem t)
    {
        playerCore.GetComponent<PlayerInventory>().CmdRequestProximityItem(t.index, t.slot, t.id, savedProximity);
    }

    //MENU INVENTORY
    void MenuInventoryRequest()
    {
        Debug.Log("Update");
        playerCore.GetComponent<PlayerInventory>().CmdRequestInventory();
    }

    public void MenuInventoryCallback(PlayerInventory.InventoryItemClient[] items)
    {
        foreach (Transform t in menuC)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in menuNC)
        {
            Destroy(t.gameObject);
        }

        for (int count = 0; count < items.Length; count++)
        {
            Item target = MasterListDatabase.i.FetchItem(items[count].ID);
            Transform type = null;
            if (target.Category != Defs.ItemCategory.Consumeable)
            {
                type = menuNC;
            }
            else
            {
                type = menuC;
            }
            GameObject slot = Instantiate(inventorySlot, type, false);
            slot.transform.GetChild(0).GetComponent<Image>().sprite = target.Image;
            slot.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = items[count].quantity.ToString("00");
        }
    }

    //MENU ABILITIES
    public void MenuRequestAbilitiesInfo()
    {
        playerCore.CmdMenuRequestAbilitiesInfo();
    }

    public void MenuCallbackAbilitiesInfo(int skillpoints, int level, float xp, float xpMax, Defs.PlayerClass playerClass, Vector3 usedSlots)
    {
        //Character
        characterMenu.alpha = 1;
        characterMenu.blocksRaycasts = true;
        characterMenu.interactable = true;

        //Class
        menuAbilities.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = playerClass.ToString();
        Vector3 availableSlots = PlayerCore.PlayerClassToSlots(playerClass);
        int workingIndex = 0;
        for (int red = 0; red < availableSlots.x; red++)
        {
            menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.red;
            if (red < usedSlots.x)
            {
                menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.red;
            }
            workingIndex++;
        }

        for (int blue = 0; blue < availableSlots.y; blue++)
        {
            menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.blue;
            if (blue < usedSlots.y)
            {
                menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.blue;
            }
            workingIndex++;
        }

        for (int green = 0; green < availableSlots.z; green++)
        {
            menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.green;
            if (green < usedSlots.z)
            {
                menuAbilities.transform.GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.green;
            }
            workingIndex++;
        }

        //Level
        menuAbilitiesLevelFill.fillAmount = xp / xpMax;
        if (level != 5)
        {
            menuAbilitiesLevelText.text = "Level " + level + " (" + Mathf.Clamp(xp, 0, xpMax).ToString("F0") + " / " + xpMax.ToString("F0") + ")";
        }
        else
        {
            menuAbilitiesLevelText.text = "Max Level";
        }

        //Points
        if (skillpoints == 0)
        {
            menuAbilitiesAP.text = "<color=#787878>0";
        }
        else
        {
            menuAbilitiesAP.text = "<color=yellow>" + skillpoints;
        }

        //Button
        if (skillpoints == 0)
        {
            menuAbilitiesLearn.interactable = false;
        }
        else
        {
            menuAbilitiesLearn.interactable = true;
        }
    }

    //ABILITIES
    public void AbilitiesRequestMenu()
    {
        playerCore.CmdAbilitiesRequestMenu();
    }

    public void AbilityCallbackMenu(Defs.PlayerClass playerClass, Vector3 usedSlots, bool[] unavailableAbilities, int[] equippedAbilities)
    {
        //Character
        characterMenu.alpha = 0;

        //Abilities
        abilityMenu.alpha = 1;
        abilityMenu.interactable = true;
        abilityMenu.blocksRaycasts = true;

        //Class
        abilityMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = playerClass.ToString();
        Vector3 availableSlots = PlayerCore.PlayerClassToSlots(playerClass);
        int workingIndex = 0;
        for (int red = 0; red < availableSlots.x; red++)
        {
            abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.red;
            if (red < usedSlots.x)
            {
                abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.red;
            }
            workingIndex++;
        }

        for (int blue = 0; blue < availableSlots.y; blue++)
        {
            abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.blue;
            if (blue < usedSlots.y)
            {
                abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.blue;
            }
            workingIndex++;
        }

        for (int green = 0; green < availableSlots.z; green++)
        {
            abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetComponent<Image>().color = Color.green;
            if (green < usedSlots.z)
            {
                abilityMenu.transform.GetChild(0).GetChild(0).GetChild(workingIndex + 1).GetChild(0).GetComponent<Image>().color = Color.green;
            }
            workingIndex++;
        }

        //Abilities
        for (int count = 0; count < unavailableAbilities.Length; count++)
        {
            Transform target = abilityMenu.transform.GetChild(0).GetChild(2).GetChild(count).GetChild(0);

            if (unavailableAbilities[count])
            {
                target.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                target.GetComponent<AbilityUpgradeSlot>().canUse = false;
            }
            else
            {
                target.GetComponent<Image>().color = Color.white;
                target.GetComponent<AbilityUpgradeSlot>().canUse = true;
            }
        }

        //ActionBar
        for (int count = 0; count < equippedAbilities.Length; count++)
        {
            Transform target = abilityMenu.transform.GetChild(0).GetChild(4).GetChild(count);
            if (equippedAbilities[count] == -1)
            {
                target.GetChild(0).GetComponent<Image>().sprite = abilityDefault;
            }
            else
            {
                target.GetChild(0).GetComponent<Image>().sprite = abilitySheet.contained[equippedAbilities[count]].image;
                target.GetChild(1).GetComponent<Animator>().SetTrigger("Ready");
            }
        }
    }

    public void AbilityMenuClose()
    {
        //Update Elements
        MenuRequestAbilitiesInfo();

        //Character
        characterMenu.alpha = 1;

        //Abilities
        abilityMenu.alpha = 0;
        abilityMenu.interactable = false;
        abilityMenu.blocksRaycasts = false;
    }

    public IEnumerator Fade(float fadeVal)
    {
        float elapsedTime = 0;
        float startVal = root.alpha;
        while (elapsedTime < 0.2f)
        {
            root.alpha = Mathf.Lerp(startVal, fadeVal, (elapsedTime / 0.2f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
