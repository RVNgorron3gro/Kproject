using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_SystemMenu : MonoBehaviour
{
    [HideInInspector] public static UI_SystemMenu i;

    CanvasGroup gameGroup;

    [Header("Root")]
    public bool root_Active;
    GameObject root_Main;

    [Header("Options")]
    public bool options_Active;
    GameObject options_Main;
    public OptionsSelection options_Selection = OptionsSelection.None;
    public enum OptionsSelection
    {
        None, Game, Keybindings, Video, Audio
    }
    public Color options_SelectedColor;

    public Transform options_Game;
    public Image options_GameI;

    public Transform options_Keybinds;
    public Image options_KeybindsI;
    public GameObject keybinds_Object;

    public Transform options_Video;
    public Image options_VideoI;

    public Transform options_Audio;
    public Image options_AudioI;
    public AudioMixer options_AudioMixer;
    public bool options_AudioPreventChange;

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
        gameGroup = GameObject.Find("UI/System").GetComponent<CanvasGroup>();

        //Root References
        root_Main = GameObject.Find("UI/System/Root");

        //Options References
        options_Main = GameObject.Find("UI/System/Options");

        options_Game = GameObject.Find("UI/System/Options/Background/Contents/Game").transform;
        options_GameI = GameObject.Find("UI/System/Options/Background/Selection/Game").GetComponent<Image>();

        options_Keybinds = GameObject.Find("UI/System/Options/Background/Contents/Keybindings").transform;
        options_KeybindsI = GameObject.Find("UI/System/Options/Background/Selection/Keybindings").GetComponent<Image>();

        options_Video = GameObject.Find("UI/System/Options/Background/Contents/Video").transform;
        options_VideoI = GameObject.Find("UI/System/Options/Background/Selection/Video").GetComponent<Image>();

        options_Audio = GameObject.Find("UI/System/Options/Background/Contents/Audio").transform;
        options_AudioI = GameObject.Find("UI/System/Options/Background/Selection/Audio").GetComponent<Image>();

        //Initialise Selection
        gameGroup.alpha = 0;
        gameGroup.interactable = gameGroup.blocksRaycasts = false;

        root_Main.SetActive(false);

        options_Game.gameObject.SetActive(false);
        options_Keybinds.gameObject.SetActive(false);
        options_Video.gameObject.SetActive(false);
        options_Audio.gameObject.SetActive(false);
        options_Main.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !UI_Chat.i.active)
        {
            HandleEscape();
        }
    }

    void HandleEscape()
    {
        if (!options_Active)
        {
            root_Active = !root_Active;
            if (root_Active)
            {
                Root_Open();
            }
            else
            {
                Root_Close();
            }
        }
        else
        {
            Root_CloseOptions();
        }
    }

    public void Root_Open()
    {
        root_Main.SetActive(true);
        gameGroup.alpha = 1;
        gameGroup.interactable = true;
        gameGroup.blocksRaycasts = true;
    }

    public void Root_Close()
    {
        root_Main.SetActive(false);
        gameGroup.alpha = 0;
        gameGroup.interactable = false;
        gameGroup.blocksRaycasts = false;
    }

    public void Root_OpenOptions()
    {
        root_Main.SetActive(false);
        options_Main.SetActive(true);
        options_Active = true;
        root_Active = false;
    }

    public void Root_CloseOptions()
    {
        root_Main.SetActive(true);
        options_Main.SetActive(false);
        options_Active = false;
        root_Active = true;
    }

    public void Options_SelectNewMenu(int newSelection)
    {
        //Reset Selection
        options_GameI.color = options_KeybindsI.color = options_VideoI.color = options_AudioI.color = Color.white;

        //Reset Active Modules
        options_Game.gameObject.SetActive(false);
        options_GameI.transform.GetChild(1).GetComponent<Button>().interactable = true;
        options_Keybinds.gameObject.SetActive(false);
        options_KeybindsI.transform.GetChild(1).GetComponent<Button>().interactable = true;
        options_Video.gameObject.SetActive(false);
        options_VideoI.transform.GetChild(1).GetComponent<Button>().interactable = true;
        options_Audio.gameObject.SetActive(false);
        options_AudioI.transform.GetChild(1).GetComponent<Button>().interactable = true;

        switch (newSelection)
        {
            case 0:
                options_GameI.color = options_SelectedColor;
                options_GameI.transform.GetChild(1).GetComponent<Button>().interactable = false;
                options_Selection = OptionsSelection.Game;

                options_Game.gameObject.SetActive(true);
                break;
            case 1:
                options_KeybindsI.color = options_SelectedColor;
                options_KeybindsI.transform.GetChild(1).GetComponent<Button>().interactable = false;
                options_Selection = OptionsSelection.Keybindings;
                Options_RebuildKeybinds();

                options_Keybinds.gameObject.SetActive(true);
                break;
            case 2:
                options_VideoI.color = options_SelectedColor;
                options_VideoI.transform.GetChild(1).GetComponent<Button>().interactable = false;
                options_Selection = OptionsSelection.Video;

                options_Video.gameObject.SetActive(true);
                break;
            case 3:
                options_AudioI.color = options_SelectedColor;
                options_AudioI.transform.GetChild(1).GetComponent<Button>().interactable = false;
                options_Selection = OptionsSelection.Audio;
                Options_InitialiseVolumes();

                options_Audio.gameObject.SetActive(true);
                break;
        }
    }

    void Options_RebuildKeybinds()
    {
        //First Clear It
        foreach (Transform t in options_Keybinds.GetChild(0).GetChild(0).GetChild(0))
        {
            Destroy(t.gameObject);
        }

        List<Binds.Binding> binds = Binds.i.sessionBindings;
        for (int count = 0; count < binds.Count; count++)
        {
            GameObject generatedBinding = Instantiate(keybinds_Object, options_Keybinds.GetChild(0).GetChild(0).GetChild(0), false);
            generatedBinding.name = generatedBinding.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = binds[count].name;
            generatedBinding.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = binds[count].key.ToString();
            generatedBinding.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(() => Options_RebindKey(generatedBinding.name));
        }
    }

    void Options_RebindKey(string target)
    {
        Debug.Log(target);
        Transform rebindT = options_Keybinds.GetChild(2);
        rebindT.gameObject.SetActive(true);
        Binds.Binding targetBind = Binds.i.GetBind(target);
        //Binds.Binding targetBind = Binds.i.currentBindings.contained[target];
        string message = "Rebinding [" + targetBind.name + "] \n" +
            "Press the new desired key or ESC to cancel";
        rebindT.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        Binds.i.rebind = target;
        //Binds.i.rebind = Binds.i.currentBindings.contained[target].name;
        GameObject.Find("Options").GetComponent<CanvasGroup>().interactable = false;
    }

    public void Options_FinishRebind()
    {
        Transform rebindT = options_Keybinds.GetChild(2);
        rebindT.gameObject.SetActive(false);
        GameObject.Find("Options").GetComponent<CanvasGroup>().interactable = true;
        Options_RebuildKeybinds();
    }

    void Options_InitialiseVolumes()
    {
        options_AudioPreventChange = true;
        for (int count = 0; count < 5; count++)
        {
            float volumeValue = PlayerPrefs.GetFloat("Audio_Vol" + count);
            options_AudioMixer.SetFloat("Vol" + count, -(volumeValue * 80));
            options_Audio.GetChild(count).GetChild(1).GetChild(0).GetComponent<Slider>().value = volumeValue;
            options_Audio.GetChild(count).GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (volumeValue * 100).ToString("F0");
        }
        options_AudioPreventChange = false;
    }

    public void Options_ChangeVolume(Slider sender)
    {
        if (!options_AudioPreventChange)
        {
            float newVolume = -(sender.value * 80);
            int count = -1;
            switch (sender.name)
            {
                case "0":
                    count = 0;
                    break;
                case "1":
                    count = 1;
                    break;
                case "2":
                    count = 2;
                    break;
                case "3":
                    count = 3;
                    break;
                case "4":
                    count = 4;
                    break;
            }
            options_AudioMixer.SetFloat("Vol" + count, newVolume);
            options_Audio.GetChild(count).GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = (sender.value * 100).ToString("F0");
            Options_SaveVolumes();
        }
    }

    public void Options_SaveVolumes()
    {
        for (int count = 0; count < 5; count++)
        {
            float volumeValue;
            options_AudioMixer.GetFloat("Vol" + count.ToString(), out volumeValue);
            volumeValue = Mathf.Round(((-volumeValue / 80)) * 100) / 100;
            PlayerPrefs.SetFloat("Audio_Vol" + count, volumeValue);
        }
    }

    public void Options_RestoreDefaultKeybinds()
    {
        Binds.i.RebuildSessionBindings();
        Options_RebuildKeybinds();
    }
}