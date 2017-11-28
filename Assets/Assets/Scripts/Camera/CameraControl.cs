using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

public class CameraControl : MonoBehaviour
{
    public static CameraControl i;
    Camera view;

    [Header("References")]
    public Transform target;
    public Transform originalTarget;
    public FieldOfView fov;
    PostProcessingBehaviour pBehaviour;

    [Header("Profiles")]
    public PostProcessingProfile normal;
    public PostProcessingProfile death;

    [Header("Settings")]
    public const float defaultHeight = 220;
    public bool follow = false;

    [Header("Targeting")]
    private Vector3 targetPosition;
    public float targetSmoothTime = 1.5f;
    public Vector3 targetVelocity;

    [Header("Zoom")]
    public float zoom;
    public float zoomSpeed = 4;
    public float zoomMin = 30;
    public float zoomMax = 75;

    [Header("Offset")]
    public Vector3 customOffset;
    public Vector3 offset;
    public float offsetMagnitude = 0.1f;
    public float addedZ = 5.5f;

    [Header("View")]
    public bool topAngle = false;

    void Awake()
    {
        i = this;   
    }

    void Start()
    {
        view = GetComponent<Camera>();
        pBehaviour = GetComponent<PostProcessingBehaviour>();
    }

    void Update()
    {
        //Call functions
        if (target)
        {
        if (fov == null && target)
            fov = target.GetComponent<FieldOfView>();

            SetAngle();
            FollowTarget();
            Zoom();
        }
    }

    void SetAngle()
    {
        if ((topAngle == true) && (addedZ != 0))
        {
            addedZ = 0;
        }
        else if ((topAngle == false) && (addedZ != 5.5f))
        {
            addedZ = 5.5f;
        }
    }

    void FollowTarget()
    {
        //Get position
        targetPosition = new Vector3(target.position.x, defaultHeight, target.position.z - addedZ);

        //Introduce Offset
        if (Input.mousePresent)
        {
            offset = view.ScreenToViewportPoint(Input.mousePosition) * offsetMagnitude;
            offset.x = Mathf.Clamp(offset.x, 0, offsetMagnitude);
            offset.y = Mathf.Clamp(offset.y, 0, offsetMagnitude);
        }
        else
        {
            offset = Vector3.zero;
        }

        targetPosition.x += -(offsetMagnitude / 2) + offset.x;
        targetPosition.z += -(offsetMagnitude / 2) + offset.y;

        //targetPosition += customOffset;

        //Set the new position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.root.position, targetPosition, ref targetVelocity, targetSmoothTime);
        transform.root.position = smoothedPosition;

        //Now Update FOV
        fov.DrawFieldOfView();
    }

    void Zoom()
    {
        zoom = Mathf.Clamp(zoom - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, zoomMin, zoomMax);
        view.fieldOfView = Mathf.Lerp(view.fieldOfView, zoom, 0.15f);
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            follow = true;
        }
        else
        {
            follow = false;
        }
    }

    public void ChangeProfile(Defs.PostProcessingProfile target)
    {
        switch (target)
        {
            case Defs.PostProcessingProfile.Normal:
                pBehaviour.profile = normal;
                break;
            case Defs.PostProcessingProfile.Death:
                pBehaviour.profile = death;
                break;
        }
    }

    public IEnumerator TargetTemp(Transform tempTarget, float time)
    {
        target = tempTarget;
        zoom = zoomMin;
        offsetMagnitude = 0;
        yield return new WaitForSeconds(time);
        target = originalTarget;
        zoom = zoomMax;
        offsetMagnitude = 6;
    }

    public void IsInUI(bool to)
    {
        if (to)
        {
            follow = false;
            offsetMagnitude = 0;
        }
        else
        {
            follow = true;
            offsetMagnitude = 6;
        }

        //Methods
        target.GetComponent<WeaponCore>().IsInUI(to);
    }
}