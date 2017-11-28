using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DevPhase : MonoBehaviour
{
    public string version;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = version;
        StartCoroutine(DelayedUpdate());
    }

    IEnumerator DelayedUpdate()
    {
        while (true)
        {
            if (NetworkManager.singleton.IsClientConnected())
            {
                GetComponent<TextMeshProUGUI>().text = version + "\n<size=14>" +
                (1.0f / Time.unscaledDeltaTime).ToString("F1") + " FPS | " + NetworkManager.singleton.client.GetRTT().ToString("F1") + "ms";
            }
            yield return new WaitForSeconds(1);
        }
    }
}