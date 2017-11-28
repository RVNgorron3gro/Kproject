using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Message : MonoBehaviour
{
    [HideInInspector] public static UI_Message i;

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

    public void Show(string message, Defs.MessageType type)
    {
        TextMeshProUGUI container = GameObject.Find("UI/HUD/Message").GetComponent<TextMeshProUGUI>();
        Color targetColor;
        switch (type)
        {
            case Defs.MessageType.Notify:
                targetColor = Color.cyan;
                break;
            case Defs.MessageType.Warn:
                targetColor = Color.red;
                break;
            default:
                targetColor = Color.white;
                break;
        }
        container.color = targetColor;
        container.text = message;

        StartCoroutine(Clear());
    }

    public IEnumerator Clear()
    {
        TextMeshProUGUI container = GameObject.Find("UI/HUD/Message").GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(2);
        container.color = Color.clear;
    }
}