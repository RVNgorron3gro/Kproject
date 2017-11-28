using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TroopHUD : MonoBehaviour
{
    [Header("References")]
    public Transform cameraT;

    [Header("UI")]
    public Image healthBar;
    public TextMeshProUGUI memberCount;

    void Start()
    {
        cameraT = Camera.main.transform;
        healthBar = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        memberCount = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

    }

    /*
    void Update()
    {

    }
    */

    /*
    void LateUpdate()
    {
        transform.LookAt(cameraT);
    }
    */

    public void RefreshHUD(float[] memberHealth, float memberMaxHealth)
    {
        //Get member count
        int count = 0;

        //Get PCT
        float PCT = 0;
        for (int member = 0; member < 5; member++)
        {
            //Member Count
            if (memberHealth[member] != 0)
            {
                count++;
            }

            //PCT
            PCT += memberHealth[member] / memberMaxHealth;
        }
        PCT /= 5;

        memberCount.text = count.ToString();
        healthBar.fillAmount = PCT;
    }
}
