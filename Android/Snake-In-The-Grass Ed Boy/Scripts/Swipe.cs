using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange;
    public float tapRange;
/*
    public void Swipes()
    {
        //0 refers to the first input on the screen. If there is a touch and the touch phase is beginning,
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //then set the start touch position to where the finger touched the screen 
            startTouchPosition = Input.GetTouch(0).position;
        }
        //If there is a touch and the finger has moved
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            //Get the current position of the finger
            //and subtract that by the fingers start position
            //to get the distance traveled
            currentPosition = Input.GetTouch(0).position;
            Vector2 Distance = currentPosition - startTouchPosition;

            if (!stopTouch)
            {
                
            }
        }
    }
    */
}
