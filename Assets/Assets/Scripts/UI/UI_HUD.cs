using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
    [HideInInspector] public static UI_HUD i;

    public UnitCore target;
    public PlayerCore playerCore;
    public int targetID;

    [Header("Border Effects")]
    public Image border_Damage;

    [Header("Elements")]
    public Transform elementRoot;
    public GameObject elementObject;
    [System.Serializable]
    public class ContainedElement
    {
        public Transform root;
        public Defs.ResourceTypes resource;
        public Image fill;
        public Image fillReduction;
        public TextMeshProUGUI text;
        public float throwVal;
        public Transfer lastTransfer;
        public float timeout;
    }
    public List<ContainedElement> elements = new List<ContainedElement>();

    [Header("States")]
    public Transform stateParent;
    public GameObject stateObject;

    [Header("Capture")]
    public CanvasGroup capture_CanvasAlpha;
    public Image capture_Progress;
    public TextMeshProUGUI capture_HelpText;
    public Coroutine capture_Instance;
    public bool capture_Yield;

    [Header("Location")]
    public TextMeshProUGUI location_Name;
    public Coroutine location_Instance;

    [Header("Turn")]
    public Animator turn_Anim;
    public TextMeshProUGUI turn_Text;
    public AudioClip turn_ClipTick;

    [Header("Action Bar")]
    public int targetSlot;
    public Abilities abilitySheet;
    public Sprite abilityDefault;
    public Transform actionBarT;
    public GameObject actionBarSPA;

    [Header("Hot Bar")]
    public Transform hotBar_Parent;
    public GameObject hotBar_Object;
    public List<HotBarContainer> hotBarC;
    public struct HotBarContainer
    {
        public Image sprite;
        public TextMeshProUGUI quantity;
    }

    [Header("Scoreboard")]
    public CanvasGroup scoreboard;

    [Header("Respawn")]
    public GameObject deathRoot;
    public GameObject deathParticles;
    public ParticleSystem deathParticleSystem;
    public Animator deathAnim;
    public AudioSource deathSource;
    public TextMeshProUGUI deathTimer;

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
        //Border Effects
        border_Damage = GameObject.Find("HUD/BorderEffects/Damage").GetComponent<Image>();

        //State
        stateParent = GameObject.Find("HUD/UpperLeft/States").transform;

        //Capture
        capture_CanvasAlpha = GameObject.Find("HUD/Capture").GetComponent<CanvasGroup>();
        capture_Progress = GameObject.Find("HUD/Capture/Progress").GetComponent<Image>();
        capture_HelpText = GameObject.Find("HUD/Capture/Text").GetComponent<TextMeshProUGUI>();

        //Location
        location_Name = GameObject.Find("Location/Text").GetComponent<TextMeshProUGUI>();

        //Turn
        turn_Anim = GameObject.Find("Turn/Border").GetComponent<Animator>();
        turn_Text = GameObject.Find("Turn/Border/Text").GetComponent<TextMeshProUGUI>();

        //Action Bar
        actionBarT = GameObject.Find("HUD/ActionBar").transform;
        actionBarSPA = actionBarT.GetChild(5).gameObject;

        //Hot Bar
        hotBar_Parent = GameObject.Find("HUD/HotBar").transform;

        //Scoreboard
        //scoreboard = GameObject.Find("HUD/Scoreboard").GetComponent<CanvasGroup>();

        //Respawn
        deathRoot = GameObject.Find("HUD/Death");
        deathParticles = GameObject.Find("DeathParticles");
        deathParticleSystem = deathParticles.GetComponent<ParticleSystem>();
        deathAnim = GameObject.Find("HUD/Death/Counter").GetComponent<Animator>();
        deathSource = GameObject.Find("HUD/Death/Counter/Tick").GetComponent<AudioSource>();
        deathTimer = GameObject.Find("HUD/Death/Timer").GetComponent<TextMeshProUGUI>();

        //Init Respawn
        deathRoot.SetActive(false);
        deathParticles.SetActive(false);
    }

    void Update()
    {
        if (!target)
            return;

        /*
        //Scoreboard
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.alpha = 1;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.alpha = 0;
        }
        */

        //Border
        border_Damage.color = Color.Lerp(border_Damage.color, Color.clear, 4 * Time.deltaTime);

        for (int count = 0; count < elements.Count; count++)
        {
            elements[count].throwVal = Mathf.Lerp(elements[count].throwVal, elements[count].lastTransfer.Val, 6 * Time.deltaTime);
            if (elements[count].text != null)
                elements[count].text.text = elements[count].throwVal.ToString("F0") + "/<b>" + elements[count].lastTransfer.Max.ToString("F0") + "</b>";

            if (elements[count].timeout == 0)
            {
                elements[count].fillReduction.fillAmount = Mathf.Lerp(elements[count].fillReduction.fillAmount, elements[count].fill.fillAmount, 5 * Time.deltaTime);
            }
            else
            {
                elements[count].timeout = Mathf.Clamp(elements[count].timeout - Time.deltaTime, 0, 1.25f);

                if (elements[count].fillReduction.fillAmount < elements[count].fill.fillAmount)
                {
                    elements[count].fillReduction.fillAmount = elements[count].fill.fillAmount;
                }
            }
        }

        /*
		//Health
		health = Mathf.Lerp(health, target.health_Val, 0.2f);
		health_Text.text = health.ToString("F0") + "/<b>" + target.health_Max.ToString("F0") + "</b>";

		if (health_ReductionTimeout == 0)
		{
			health_ReductionBar.fillAmount = Mathf.Lerp(health_ReductionBar.fillAmount, health_Bar.fillAmount, 0.05f);
		}
		else
		{
			health_ReductionTimeout = Mathf.Clamp(health_ReductionTimeout - Time.deltaTime, 0, 1.2f);

			if (health_ReductionBar.fillAmount < health_Bar.fillAmount)
			{
				health_ReductionBar.fillAmount = health_Bar.fillAmount;
			}
		}

		//Stamina
		stamina = Mathf.Lerp(stamina, target.stamina_Val, 0.2f);
		stamina_Text.text = stamina.ToString("F0") + "/<b>" + target.stamina_Max.ToString("F0") + "</b>";

		if (stamina_ReductionTimeout == 0)
		{
			stamina_ReductionBar.fillAmount = Mathf.Lerp(stamina_ReductionBar.fillAmount, stamina_Bar.fillAmount, 0.05f);
		}
		else
		{
			stamina_ReductionTimeout = Mathf.Clamp(stamina_ReductionTimeout - Time.deltaTime, 0, 1.2f);

			if (stamina_ReductionBar.fillAmount < stamina_Bar.fillAmount)
			{
				stamina_ReductionBar.fillAmount = stamina_Bar.fillAmount;
			}
		}
		*/
    }

    public void AddResource(Defs.ResourceTypes target, float max)
    {
        GameObject newSpawn = Instantiate(elementObject, elementRoot, false);
        ContainedElement newElement = new ContainedElement
        {
            root = newSpawn.transform,
            resource = target,
            fill = newSpawn.transform.GetChild(0).GetChild(0).GetComponent<Image>(),
            fillReduction = newSpawn.transform.GetChild(0).GetComponent<Image>(),
            text = newSpawn.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(),
            lastTransfer = new Transfer(max, max, 1),
            throwVal = max,
            timeout = 0,
        };

        Color color = Color.black;
        Defs.LayoutSize size = Defs.LayoutSize.Medium;
        switch (target)
        {
            case Defs.ResourceTypes.Health:
                color = Parameters.i.Health.Color;
                size = Parameters.i.Health.pHBar;
                break;
            case Defs.ResourceTypes.Stamina:
                color = Parameters.i.Stamina.Color;
                size = Parameters.i.Stamina.pHBar;
                break;
            case Defs.ResourceTypes.Mana:
                color = Parameters.i.Mana.Color;
                size = Parameters.i.Mana.pHBar;
                break;
            case Defs.ResourceTypes.Bloodlust:
                color = Parameters.i.Bloodlust.Color;
                size = Parameters.i.Bloodlust.pHBar;
                break;
            case Defs.ResourceTypes.Sunlight:
                color = Parameters.i.Sunlight.Color;
                size = Parameters.i.Sunlight.pHBar;
                break;
            case Defs.ResourceTypes.Moonlight:
                color = Parameters.i.Moonlight.Color;
                size = Parameters.i.Moonlight.pHBar;
                break;
            case Defs.ResourceTypes.Curse:
                color = Parameters.i.Curse.Color;
                size = Parameters.i.Curse.pHBar;
                break;
            case Defs.ResourceTypes.Corruption:
                color = Parameters.i.Corruption.Color;
                size = Parameters.i.Corruption.pHBar;
                break;
            case Defs.ResourceTypes.Darkness:
                color = Parameters.i.Darkness.Color;
                size = Parameters.i.Darkness.pHBar;
                break;
        }
        newElement.fill.color = color;
        float height = 2;
        switch (size)
        {
            case Defs.LayoutSize.Large:
                height = 25;
                break;
            case Defs.LayoutSize.Medium:
                height = 18;
                break;
            case Defs.LayoutSize.Small:
                height = 10;
                newElement.text = null;
                break;
        }
        newElement.root.GetComponent<LayoutElement>().minHeight = height;
        if (newElement.text != null)
            newElement.text.text = max.ToString("F0") + "/<b>" + max.ToString("F0") + "</b>";
        elements.Add(newElement);
    }

    public void UpdateResource(Defs.ResourceTypes target, Transfer vals)
    {
        for (int count = 0; count < elements.Count; count++)
        {
            if (elements[count].resource == target)
            {
                if (elements[count].lastTransfer.PCT > vals.PCT)
                {
                    elements[count].timeout = 1.25f;
                    if (target == Defs.ResourceTypes.Health)
                        BorderFlash();
                }

                elements[count].fill.fillAmount = vals.PCT;
                elements[count].lastTransfer = vals;
                break;
            }
        }
    }

    void BorderFlash()
    {
        border_Damage.color = Color.white;
    }

    /*
	public void UpdateHealth(Transfer pack)
	{
		if (health_PrevAmount > pack.PCT)
		{
			health_ReductionTimeout = 1.2f;
			BorderFlash();

		}
		else
		{
			health_ReductionTimeout = 0;
		}

		//Bar
		health_Bar.fillAmount = pack.PCT;

		//For Reduction Timeout
		health_PrevAmount = pack.PCT;
	}

	public void UpdateStamina(Transfer pack)
	{
		if (stamina_PrevAmount > pack.PCT)
		{
			stamina_ReductionTimeout = 1.2f;
		}
		else
		{
			stamina_ReductionTimeout = 0;
		}

		//Bar
		stamina_Bar.fillAmount = pack.PCT;

		//For Reduction Timeout
		stamina_PrevAmount = pack.PCT;
	}
	*/

    public void UpdateClock(int totalTurn)
    {
        turn_Text.text = totalTurn.ToString("00");
        turn_Anim.SetTrigger("Tick");
        AudioController.i.Play2DSound(turn_ClipTick, 1);
    }

    public void PingCaptureTimer(bool fail)
    {
        if (capture_Instance != null)
        {
            StopCoroutine(capture_Instance);
        }

        capture_Instance = StartCoroutine(EPingCaptureTimer(fail));
    }

    IEnumerator EPingCaptureTimer(bool fail)
    {
        capture_Yield = true;
        if (fail)
        {
            capture_HelpText.color = Color.red;
            capture_HelpText.text = "Interrupted!";
            capture_Progress.color = Color.red;
        }
        else
        {
            capture_HelpText.color = Color.green;
            capture_HelpText.text = "Captured!";
            capture_Progress.color = Color.green;
        }

        yield return new WaitForSeconds(1.75f);
        capture_Yield = false;
        capture_CanvasAlpha.alpha = 0;
    }

    public void UpdateCaptureTimer(float capturePercent)
    {
        if (!capture_Yield)
        {
            capture_CanvasAlpha.alpha = 1;
            capture_HelpText.color = Color.white;
            capture_Progress.color = Color.white;
            capture_HelpText.text = "Capturing...";
            capture_Progress.fillAmount = capturePercent;
        }
    }

    public void UpdateLocation(int owner, bool nonCore, string regionHandle, int playerID)
    {
        if (!nonCore)
        {
            if (owner == playerID)
            {
                location_Name.color = Color.green;
            }
            else if (owner == -1)
            {
                location_Name.color = Color.white;
            }
            else
            {
                location_Name.color = Color.red;
            }
        }
        else
        {
            location_Name.color = Color.grey;
        }

        location_Name.text = regionHandle;
    }

    /*
	public void PingLocation(GameObject regionObject, int playerID)
	{
		if (location_Instance != null)
		{
			StopCoroutine(location_Instance);
		}

		location_Instance = StartCoroutine(EPingLocation(regionObject, playerID));
	}

	IEnumerator EPingLocation(GameObject regionObject, int playerID)
	{
		RegionCore region = regionObject.GetComponent<RegionCore>();

		if (!region.nonCore)
		{
			if (region.owner == playerID)
			{
				location_Name.color = Color.green;
			}
			else if (region.owner == -1)
			{
				location_Name.color = Color.white;
			}
			else
			{
				location_Name.color = Color.red;
			}
		}
		else
		{
			location_Name.color = Color.gray;
		}

		location_Name.text = region.regionHandle;

		yield return new WaitForSeconds(1.75f);
		location_Name.color = Color.clear;
	}
	*/

    public void RequestStateUpdate()
    {
        target.GetComponent<HERO_StateController>().CmdRequestStateUpdate();
    }

    public void CallbackStateUpdate(StateTransfer[] activeStates)
    {
        //Clear
        foreach (Transform t in stateParent)
        {
            Destroy(t.gameObject);
        }

        //Rebuild
        for (int count = 0; count < activeStates.Length; count++)
        {
            Transform newState = Instantiate(stateObject, stateParent).transform;
            if (StateList.i.contained[count].debuff)
            {
                newState.GetComponent<Image>().color = Color.red;
            }
            newState.GetChild(0).GetComponent<Image>().sprite = StateList.i.contained[activeStates[count].StateID].image;
            float cur = activeStates[count].Duration; float max = activeStates[count].MaxDuration;
            newState.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - cur / max;
            if (activeStates[count].Stacks > 1)
            {
                newState.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = activeStates[count].Stacks.ToString();
            }
        }
    }

    /*
    public void HandleStates(List<State> states, List<HERO_StateController.Properties> properties)
    {
        //Check if the states need to be rebuilt
        bool fail = false;
        if (state_Hashes.Count == states.Count)
        {
            for (int count = 0; count < state_Objects.Count; count++)
            {
                if (state_Hashes[count] != state_Objects[count].GetHashCode())
                {
                    RebuildStates(states, properties);
                    fail = true;
                    break;
                }
            }

            List<State> groupedStates = new List<State>();
            List<HERO_StateController.Properties> groupedProperties = new List<HERO_StateController.Properties>();
            List<int> groupedInstances = new List<int>();

            for (int count = 0; count < states.Count; count++)
            {
                if (count != 0)
                {
                    if (!groupedStates.Contains(states[count]))
                    {
                        groupedStates.Add(states[count]);
                        groupedProperties.Add(properties[count]);
                        if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                        {
                            groupedInstances.Add(1);
                            //Debug.Log("Added");
                        }
                    }
                    else
                    {
                        //Debug.Log(groupedInstances.Count + " // " + (groupedStates.IndexOf(states[count]) + 1).ToString());
                        if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                        {
                            groupedInstances.Add(1);
                            //Debug.Log("Added");
                        }
                        else
                        {
                            groupedInstances[groupedStates.IndexOf(states[count])]++;
                        }
                    }
                }
                else
                {
                    groupedStates.Add(states[count]);
                    groupedProperties.Add(properties[count]);

                    //Debug.Log(groupedInstances.Count + " / " + (groupedStates.IndexOf(states[count]) + 1).ToString());
                    if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                    {
                        groupedInstances.Add(1);
                        //Debug.Log("Added");
                    }
                    else
                    {
                        groupedInstances[groupedStates.IndexOf(states[count])]++;
                    }
                }
            }

            //Now update all the timings
            for (int count = 0; count < state_Objects.Count; count++)
            {
                if (!fail)
                {
                    state_Objects[count].transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - (properties[count].duration / properties[count].durationMax);

                    /*
					string time = "ERROR";
					if (properties[count].duration >= 10)
					{
						time = properties[count].duration.ToString("F0");
					}
					else if (properties[count].duration >= 1)
					{
						time = properties[count].duration.ToString("F1");
					}
					else
					{
						time = properties[count].duration.ToString("F2");
					}
					state_Objects[count].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = time;

                    if (!states[count].independent)
                    {
                        if (states[count].maxStacks != 0 && properties[count].stacks > 1)
                        {
                            state_Objects[count].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = properties[count].stacks.ToString();
                        }
                        else
                        {
                            state_Objects[count].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                        }
                    }
                    else
                    {
                        if (groupedInstances[count] > 1)
                        {
                            state_Objects[count].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = groupedInstances[count].ToString();
                        }
                        else
                        {
                            state_Objects[count].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                        }
                    }
                }
            }
        }
        else
        {
            RebuildStates(states, properties);
        }
    }

    void RebuildStates(List<State> states, List<HERO_StateController.Properties> properties)
    {
        state_Hashes.Clear();
        state_Objects.Clear();

        List<State> groupedStates = new List<State>();
        List<HERO_StateController.Properties> groupedProperties = new List<HERO_StateController.Properties>();
        List<int> groupedInstances = new List<int>();

        for (int count = 0; count < states.Count; count++)
        {
            if (count != 0)
            {
                if (!groupedStates.Contains(states[count]))
                {
                    groupedStates.Add(states[count]);
                    groupedProperties.Add(properties[count]);
                    if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                    {
                        groupedInstances.Add(1);
                        //Debug.Log("Added");
                    }
                }
                else
                {
                    //Debug.Log(groupedInstances.Count + " // " + (groupedStates.IndexOf(states[count]) + 1).ToString());
                    if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                    {
                        groupedInstances.Add(1);
                        //Debug.Log("Added");
                    }
                    else
                    {
                        groupedInstances[groupedStates.IndexOf(states[count])]++;
                    }
                }
            }
            else
            {
                groupedStates.Add(states[count]);
                groupedProperties.Add(properties[count]);
                //Debug.Log(groupedInstances.Count + " / " + (groupedStates.IndexOf(states[count]) + 1).ToString());
                if (groupedInstances.Count < groupedStates.IndexOf(states[count]) + 1)
                {
                    groupedInstances.Add(1);
                    //Debug.Log("Added");
                }
                else
                {
                    groupedInstances[groupedStates.IndexOf(states[count])]++;
                }
            }
        }

        for (int child = 0; child < 2; child++)
        {
            for (int count = 0; count < state_Parent.GetChild(child).childCount; count++)
            {
                Destroy(state_Parent.GetChild(child).GetChild(count).gameObject);
            }
        }

        for (int count = 0; count < groupedStates.Count; count++)
        {
            GameObject newState;
            if (!groupedStates[count].debuff)
            {
                newState = Instantiate(state_Buff, state_Parent.GetChild(0), false);
            }
            else
            {
                newState = Instantiate(state_Debuff, state_Parent.GetChild(1), false);
            }

            newState.name = states[count].name;
            state_Objects.Add(newState);

            newState.GetComponent<Image>().sprite = states[count].image;

            newState.transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - (properties[count].duration / properties[count].durationMax);

            /*
			string time = "ERROR";
			if (properties[count].duration >= 10)
			{
				time = properties[count].duration.ToString("F0");
			}
			else if (properties[count].duration >= 1)
			{
				time = properties[count].duration.ToString("F1");
			}
			else
			{
				time = properties[count].duration.ToString("F2");
			}
			newState.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = time;

            if (!states[count].independent)
            {
                if (states[count].maxStacks != 0 && properties[count].stacks > 1)
                {
                    newState.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = properties[count].stacks.ToString();
                }
                else
                {
                    newState.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            else
            {
                if (groupedInstances[count] > 1)
                {
                    newState.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = groupedInstances[count].ToString();
                }
                else
                {
                    newState.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                }
            }

            //Now Update the hashes
            state_Hashes.Add(newState.GetHashCode());
        }
    }
    */

    public void UpdateActionBar(int[] ability, string[] control)
    {
        for (int count = 0; count < ability.Length; count++)
        {
            Transform targetAbility = GameObject.Find("HUD/ActionBar").transform.GetChild(count);
            if (ability[count] != -1)
            {
                targetAbility.GetChild(0).GetComponent<Image>().sprite = abilitySheet.contained[ability[count]].image;
                targetAbility.GetComponent<Image>().color = GameStatus.i.ClassColor(abilitySheet.contained[ability[count]].slotType);
            }
            else
            {
                targetAbility.GetChild(0).GetComponent<Image>().sprite = abilityDefault;
            }
            targetAbility.GetChild(2).GetComponent<TextMeshProUGUI>().text = control[count].ToString();
        }
    }

    public void UpdateAbility(int ability, Defs.AbilityMode mode)
    {
        Transform targetAbility = GameObject.Find("HUD/ActionBar").transform.GetChild(ability);
        Image image = targetAbility.GetChild(0).GetComponent<Image>();
        Animator animator = targetAbility.GetChild(1).GetComponent<Animator>();
        switch (mode)
        {
            case Defs.AbilityMode.Use:
                image.color = new Color(1, 1, 1, 0.3f);
                animator.SetTrigger("Use");
                break;
            case Defs.AbilityMode.Cooldown:
                animator.SetTrigger("Cooldown");
                break;
            case Defs.AbilityMode.Ready:
                image.color = Color.white;
                animator.SetTrigger("Ready");
                break;
            case Defs.AbilityMode.Level:
                animator.SetTrigger("Level");
                break;
            case Defs.AbilityMode.Chosen:
                animator.SetTrigger("Chosen");
                break;
            case Defs.AbilityMode.NotChosen:
                animator.SetTrigger("NotChosen");
                break;
        }
    }

    public void ChangeSPADisplay(bool to)
    {
        actionBarSPA.SetActive(to);
    }

    public void UpdateHotBar(int[] id, int[] quantity)
    {
        foreach (Transform t in hotBar_Parent)
        {
            Destroy(t.gameObject);
        }

        for (int count = 0; count < id.Length; count++)
        {
            Item target = MasterListDatabase.i.FetchItem(id[count]);
            GameObject newSlot = Instantiate(hotBar_Object, hotBar_Parent, false);
            newSlot.transform.GetChild(0).GetComponent<Image>().sprite = target.Image;
            newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Binds.i.GetBind("Item " + (count + 1)).key.ToString();

            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha0") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad0"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";//count.ToString();
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha1") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad1"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "1";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha2") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad2"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha3") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad3"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "3";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha4") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad4"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "4";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha5") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad5"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "5";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha6") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad6"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "6";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha7") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad7"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "7";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha8") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad8"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "8";
            }
            if ((Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Alpha9") || (Binds.i.GetBind("Item " + (count + 1)).key.ToString() == "Keypad9"))
            {
                newSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "9";
            }
            newSlot.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = quantity[count].ToString("00");
        }
    }

    public void InitiateDeathTimer()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        deathRoot.SetActive(true);
        deathParticles.SetActive(true);
        CameraControl camera = Camera.main.GetComponent<CameraControl>();
        camera.ChangeProfile(Defs.PostProcessingProfile.Death);
        camera.offsetMagnitude = 1;
    }

    public void UpdateDeathTimer(int time)
    {
        if (time != 0 && time != 15)
        {
            deathSource.pitch = Random.Range(1, 1.15f);
            deathSource.Play();
        }

        if (time % 2 == 0)
        {
            deathParticleSystem.Emit(600);
        }

        deathAnim.SetTrigger("Tick");
        deathTimer.text = time.ToString("F0");
    }

    public void EndDeathTimer()
    {
        deathRoot.SetActive(false);
        deathParticles.SetActive(false);
        CameraControl camera = Camera.main.GetComponent<CameraControl>();
        camera.ChangeProfile(Defs.PostProcessingProfile.Normal);
        camera.offsetMagnitude = 6;
        GetComponent<CanvasGroup>().alpha = 1;
    }
}