using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UI_State
{
    public static bool isInUI = false;

    public static void ChangeState(bool newState)
    {
        isInUI = newState;
        if (isInUI)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        CameraControl.i.IsInUI(isInUI);
    }
}