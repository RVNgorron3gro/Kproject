using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [HideInInspector]
    public static TitleController i;

    [Header("State")]
    public TitleState state;
    public enum TitleState
    {
        Home, Play, Knowledge, Options, Community
    }

    [Header("Level")]
    public Transform ovLevel;

    [Header("Navigation")]
    public TextMeshProUGUI ovPlay;
    public TextMeshProUGUI ovKnowledge;
    public TextMeshProUGUI ovOptions;
    public TextMeshProUGUI ovCommunity;
    public Color selectionColor;

    [Header("Lower")]
    public CanvasGroup ovLower;

    [Header("Time")]
    public TextMeshProUGUI ovTime;

    [Header("Character")]
    public Transform ovCharacter;
    public TextMeshProUGUI ovCharacterKills;
    public TextMeshProUGUI ovCharacterWins;
    public TextMeshProUGUI ovCharacterClass;

    [Header("Searching")]
    public Animator ovSearching;
    public TextMeshProUGUI ovSearchingTimer;
    public Coroutine ovSearchingCo;

    [Header("Play")]
    public Transform navPlay;
    public CanvasGroup navPlayCanvas;
    public CanvasGroup navPlayQueueCanvas;

    [Header("Knowledge")]
    public Transform navKnowledge;
    public CanvasGroup navKnowledgeCanvas;

    [Header("Options")]
    public Transform navOptions;
    public CanvasGroup navOptionsCanvas;

    [Header("Community")]
    public Transform navCommunity;
    public CanvasGroup navCommunityCanvas;

    [Header("Scene")]
    public Material skybox_Mat;
    public float skybox_Speed;

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
        //Overlay
        ovLevel = transform.GetChild(4).GetChild(1);

        //Navigation
        ovPlay = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        ovKnowledge = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        ovOptions = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        ovCommunity = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();

        //Lower
        ovLower = transform.GetChild(4).GetChild(2).GetComponent<CanvasGroup>();

        //Time
        ovTime = transform.GetChild(4).GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        //Character
        ovCharacter = transform.GetChild(4).GetChild(2).GetChild(1);
        ovCharacterKills = ovCharacter.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        ovCharacterWins = ovCharacter.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        ovCharacterClass = ovCharacter.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

        //Searching
        ovSearching = transform.GetChild(4).GetChild(3).GetComponent<Animator>();
        ovSearchingTimer = ovSearching.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        //Play
        navPlay = transform.GetChild(0);
        navPlayCanvas = navPlay.GetComponent<CanvasGroup>();
        navPlayQueueCanvas = navPlay.GetChild(0).GetComponent<CanvasGroup>();

        //Knowledge
        navKnowledge = transform.GetChild(1);
        navKnowledgeCanvas = navKnowledge.GetComponent<CanvasGroup>();

        //Options
        navOptions = transform.GetChild(2);
        navOptionsCanvas = navOptions.GetComponent<CanvasGroup>();

        //Community
        navCommunity = transform.GetChild(3);
        navCommunityCanvas = navCommunity.GetComponent<CanvasGroup>();

        //Scene
        CameraShaker.Instance.StartShake(1, 0.2f, 0);

        //At Start
        AtStart(Random.Range(0, 12));

        //Very Late Update
        StartCoroutine(VeryLateUpdate());
    }

    void AtStart(int roll)
    {
        ovCharacterClass.text = "The " + ((Defs.PlayerClass)roll).ToString();
    }

    void Update()
    {
        //Scene
        skybox_Mat.SetFloat("_Rotation", Mathf.Repeat(skybox_Mat.GetFloat("_Rotation") + (skybox_Speed * Time.deltaTime), 360));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(0);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GameObject.Find("Title").GetComponent<CanvasGroup>().alpha == 0)
            {
                GameObject.Find("Title").GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                GameObject.Find("Title").GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }

    IEnumerator VeryLateUpdate()
    {
        while (true)
        {
            //Time
            ovTime.text = System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00");

            //Now wait
            yield return new WaitForSeconds(30);
        }
    }

    public void ChangeState(int to)
    {
        TitleState converted = (TitleState)to;

        //Init
        InitialiseAllCanvases();
        InitialiseAllNavs();

        switch (converted)
        {
            case TitleState.Home:
                break;
            case TitleState.Play:
                if (ovSearchingCo == null)
                {
                    ovPlay.color = selectionColor;
                }
                break;
            case TitleState.Knowledge:
                ovKnowledge.color = selectionColor;
                break;
            case TitleState.Options:
                ovOptions.color = selectionColor;
                break;
            case TitleState.Community:
                ovCommunity.color = selectionColor;
                break;
        }
        ChangeCanvas(converted, true);
    }

    public void Search(bool start)
    {
        if (ovSearchingCo != null)
        {
            StopCoroutine(ovSearchingCo);
            ovSearchingCo = null;
        }

        if (start)
        {
            ovSearching.SetBool("Searching", true);
            ovSearchingCo = StartCoroutine(Searching());
            navPlayCanvas.alpha = navPlayQueueCanvas.alpha = 0;
            navPlayCanvas.blocksRaycasts = navPlayQueueCanvas.blocksRaycasts = navPlayCanvas.interactable = navPlayQueueCanvas.interactable = false;
            ovPlay.color = Color.gray;
        }
        else
        {
            ovSearching.SetBool("Searching", false);
            ovPlay.color = selectionColor;
        }
            ChangeState(0);
    }

    IEnumerator Searching()
    {
        float secs = 0;
        float mins = 0;
        while (true)
        {
            ovSearchingTimer.text = mins.ToString("00") + ":" + secs.ToString("00");
            yield return new WaitForSeconds(1);
            if (secs >= 59)
            {
                mins++;
                secs = 0;
            }
            else
            {
                secs++;
            }
        }
    }

    //Utility
    void ChangeCanvas(TitleState state, bool active)
    {
        float alpha = 0;
        if (active)
            alpha = 1;

        switch (state)
        {
            case TitleState.Home:
                break;
            case TitleState.Play:
                if (ovSearchingCo == null)
                {
                    navPlayCanvas.alpha = navPlayQueueCanvas.alpha = alpha;
                    navPlayCanvas.blocksRaycasts = navPlayQueueCanvas.blocksRaycasts = navPlayCanvas.interactable = navPlayQueueCanvas.interactable = active;
                }
                break;
            case TitleState.Knowledge:
                navKnowledgeCanvas.alpha = alpha;
                navKnowledgeCanvas.blocksRaycasts = navKnowledgeCanvas.interactable = active;
                break;
            case TitleState.Options:
                navOptionsCanvas.alpha = alpha;
                navOptionsCanvas.blocksRaycasts = navOptionsCanvas.interactable = active;
                break;
            case TitleState.Community:
                navCommunityCanvas.alpha = alpha;
                navCommunityCanvas.blocksRaycasts = navCommunityCanvas.interactable = active;
                break;
        }

        //Hide Lower
        if (state != TitleState.Home)
        {
            ovLower.alpha = 0;
        }
        else
        {
            ovLower.alpha = 1;
        }
    }

    void InitialiseAllCanvases()
    {
        //Play
        if (ovSearchingCo == null)
        {
            navPlayCanvas.alpha = 0;
            navPlayCanvas.interactable = false;
            navPlayCanvas.blocksRaycasts = false;
        }

        //Knowledge
        navKnowledgeCanvas.alpha = 0;
        navKnowledgeCanvas.interactable = false;
        navKnowledgeCanvas.blocksRaycasts = false;

        //Options
        navOptionsCanvas.alpha = 0;
        navOptionsCanvas.interactable = false;
        navOptionsCanvas.blocksRaycasts = false;

        //Community
        navCommunityCanvas.alpha = 0;
        navCommunityCanvas.interactable = false;
        navCommunityCanvas.blocksRaycasts = false;

    }

    void InitialiseAllNavs()
    {
        //Play
        if (ovSearchingCo == null)
            ovPlay.color = Color.white;

        //Knowledge
        ovKnowledge.color =

        //Options
        ovOptions.color =

        //Community
        ovCommunity.color = Color.white;
    }
}