using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Chat : MonoBehaviour
{
    public static UI_Chat i;
    [Header("Audio")]
    public AudioClip clipChatTick;

    TextMeshProUGUI container;
    ScrollRect rect;
    TMP_InputField input;
    CanvasGroup alpha;
    IEnumerator closeCommand;
    public bool active = false;
    bool changeLock = false;

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
        container = GameObject.Find("ChatRoot").transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        rect = GameObject.Find("ChatRoot").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<ScrollRect>();
        input = GameObject.Find("ChatRoot").transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TMP_InputField>();
        alpha = GameObject.Find("ChatRoot").transform.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (!changeLock)
        {
            if (Input.GetKeyUp(KeyCode.Return) && !UI_SystemMenu.i.root_Active)
            {
                if (!active)
                {
                    OpenChat();
                }
                else
                {
                    Send(input);
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape) && active)
            {
                CloseChat();
            }
        }
    }

    void OpenChat()
    {
        if (closeCommand != null)
        {
            StopCoroutine(closeCommand);
        }

        alpha.alpha = 1;
        alpha.interactable = true;
        alpha.blocksRaycasts = true;
        input.Select();
        active = true;
    }

    void CloseChat()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
        if (closeCommand != null)
        {
            StopCoroutine(closeCommand);
        }

        alpha.alpha = 0;
        alpha.interactable = false;
        alpha.blocksRaycasts = false;
        active = false;
    }

    public void Send(TMP_InputField message)
    {
        StartCoroutine(ForceClose());
        if (message.text.Length != 0)
        {
            Player.i.CmdSendMessage(message.text);
            message.text = "";
        }
    }

    public void AddMessageToChat(string message, string sender)
    {
        string assembledMessage;
        Color highlight = Color.blue;
        if (sender != Player.i.steamName)
        {
            highlight = UI_Styles.i.resources.hostility;
        }
        assembledMessage = "<color=#" + ColorUtility.ToHtmlStringRGB(highlight) + ">" + sender + "</color>: " + message;
        container.text += assembledMessage + "\n";
        StartCoroutine(ScrollDown());

        //Sound Effect!
        AudioController.i.Play2DSound(clipChatTick, 1);

        if (!active)
        {
            if (closeCommand != null)
            {
                StopCoroutine(closeCommand);
            }

            closeCommand = CloseDelay();
            StartCoroutine(closeCommand);
        }
    }

    public void AddSystemMessageToChat(string message)
    {
        string assembledMessage;
        assembledMessage = "<color=orange>" + message + "</color>";
        container.text += assembledMessage + "\n";
        StartCoroutine(ScrollDown());

        //Sound Effect!
        AudioController.i.Play2DSound(clipChatTick, 1, 1.25f, 1.25f);

        if (!active)
        {
            if (closeCommand != null)
            {
                StopCoroutine(closeCommand);
            }

            closeCommand = CloseDelay();
            StartCoroutine(closeCommand);
        }
    }

    IEnumerator ScrollDown()
    {
        yield return new WaitForEndOfFrame();
        if (rect != null)
        {
            rect.normalizedPosition = new Vector2(0, 0);
        }
    }

    IEnumerator CloseDelay()
    {
        alpha.alpha = 0.01f;
        yield return new WaitForSeconds(4);
        alpha.alpha = 0;
    }

    IEnumerator ForceClose()
    {
        changeLock = true;
        CloseChat();
        yield return new WaitForSeconds(0.05f);
        changeLock = false;
    }
}