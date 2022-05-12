using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Input
{
    public KeyCode primary;
    public KeyCode alternate;

    public bool Pressed()
    {
        return UnityEngine.Input.GetKey(primary) || UnityEngine.Input.GetKey(alternate);
    }
    public bool PressedDown()
    {
        return UnityEngine.Input.GetKeyDown(primary) || UnityEngine.Input.GetKeyDown(alternate);
    }
    public bool PressedUp()
    {
        return UnityEngine.Input.GetKeyUp(primary) || UnityEngine.Input.GetKeyUp(alternate);
    }
}
public class Comp_PlayerInput : MonoBehaviour
{

    public Input Forward;
    public Input Backward;
    public Input Left;
    public Input Right;
    public Input Sprint;
    public Input LockOn;
    public Input Crouch;
    public Input Prone;
    public Input Attack;

    //Gets the axis value of the forward input
    public int MoveAxisForwardRaw
    {
        get
        {
            if (Forward.Pressed() && Backward.Pressed()) { return 0; }
            if (Forward.Pressed()) { return 1; }
            if (Backward.Pressed()) { return -1; }

            return 0;
        }
    } //Gets the axis value of the right input
    public int MoveAxisRightRaw
    {
        get
        {
            if (Right.Pressed() && Left.Pressed()) { return 0; }
            if (Right.Pressed()) { return 1; }
            if (Left.Pressed()) { return -1; }

            return 0;
        }
    }


    //References the Mouse X and Y axes and Mouse ScrollWheel in project settings
    public const string MouseXString = "Mouse X";
    public const string MouseYString = "Mouse Y";
    public const string MouseScrollString = "Mouse ScrollWheel";

    //Gets the input of the X and Y and Scroll Wheel
    public static float MouseXInput { get => UnityEngine.Input.GetAxis(MouseXString); }
    public static float MouseYInput { get => UnityEngine.Input.GetAxis(MouseYString); }
    public static float MouseScrollInput { get => UnityEngine.Input.GetAxis(MouseScrollString); }


}
