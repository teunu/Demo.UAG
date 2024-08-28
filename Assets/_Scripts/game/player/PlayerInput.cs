using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput broadcast;
    public bool cursor_locked = true;

    public Vector2 move { get; private set; }
    public bool jump { get; private set; }
    public bool crouch { get; private set; }

    private void Awake()
    {
        Globals.GetGlobals();
    }

    void OnMove(InputValue input)
    {
        move = input.Get<Vector2>();
    }

    void OnJump(InputValue input)
    {
        jump = input.isPressed;
    }

    void OnCrouch(InputValue input)
    {
        crouch = input.isPressed;
    }

    #region Focus
    private void OnApplicationFocus(bool hasFocus)
    {
        CursorInquiry();
    }

    public static void CursorInquiry()
    {
        if (!InInteractionMenus && broadcast != null)
        {
            SetCursorState(broadcast.cursor_locked);
            Cursor.visible = false;
        }
        else         //In menus, on application focus should not lock or hide the thingymabob.
        {
            SetCursorState(false);
            Cursor.visible = true;
        }
    }

    private static void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public static bool InInteractionMenus
    {
        get
        {
            if (SC_Pause.instance != null) { return true; }
            //if (ScreenCharacterMediator.instance != null) { return true; }
            //if (ScreenContainerShareMediator.instance != null) { return true; }

            return false;
        }
    }
    #endregion

    #region Menus
    void OnEscape(InputValue input)
    {
        if (input.isPressed)
            SC_Pause.Open();
    }
    #endregion
}
