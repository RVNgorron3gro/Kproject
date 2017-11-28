using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    Camera cam;

    public Vector3 pos;
    public Transform target;
    public Vector3 deviation;
    public float refSpeed;
    public float smoothing;

    void Start()
    {
        cam = Camera.main;
        pos = target.position;
        Destroy(gameObject, 1.5f);
    }

    void LateUpdate()
    {
        
        deviation += transform.forward * refSpeed;
        Vector3 targetPos = pos + deviation;
        targetPos = cam.WorldToScreenPoint(targetPos);
        transform.position = targetPos;
        refSpeed = Mathf.Clamp(Mathf.Lerp(refSpeed, 0, smoothing * Time.unscaledDeltaTime), 0.005f, 100);
    }
}