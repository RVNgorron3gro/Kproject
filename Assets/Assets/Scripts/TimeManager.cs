using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager i;

    public float slowdownFactor = 0.05f;
    public float returnLength = 2;
    public float delay;
    public bool update;

    void OnEnable()
    {
        i = this;
    }

    public void StartSlowMotion(UnitCore victim)
    {
        update = false;
        StartCoroutine(CameraControl.i.TargetTemp(victim.transform, delay * 0.75f));
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(delay);
        update = true;
    }

    void Update()
    {
        if (update)
        {
            Time.timeScale += (1 / returnLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}