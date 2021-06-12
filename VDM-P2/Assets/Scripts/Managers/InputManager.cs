using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    public enum InputType { NONE, S_UP, S_DOWN, S_RIGHT, S_LEFT, TAP};

    void Start()
    {
        dragDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
    }

    void Update()
    {
        // if we're in editor, use PC input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            //save first touch 2d point
            fp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save last touch 2d point
            lp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);


            CheckSwipes();
        }
#endif

        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list

                CheckSwipes();
            }
        }
    }



    void CheckSwipes()
    {
        InputManager.InputType it = InputType.NONE;

        //Check if drag distance is greater than 20% of the screen height
        if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
        {//It's a drag
         //check if the drag is vertical or horizontal
            if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
            {   //If the horizontal movement is greater than the vertical movement...
                if ((lp.x > fp.x))  //If the movement was to the right)
                {   //Right swipe
                    Debug.Log("Right Swipe");
                    it = InputType.S_RIGHT;
                }
                else
                {   //Left swipe
                    Debug.Log("Left Swipe");
                    it = InputType.S_LEFT;
                }
            }
            else
            {   //the vertical movement is greater than the horizontal movement
                if (lp.y > fp.y)  //If the movement was up
                {   //Up swipe
                    Debug.Log("Up Swipe");
                    it = InputType.S_UP;
                }
                else
                {   //Down swipe
                    Debug.Log("Down Swipe");
                    it = InputType.S_DOWN;
                }
            }
        }
        else
        {   //It's a tap as the drag distance is less than 20% of the screen height
            Debug.Log("Tap");
            it = InputType.TAP;
        }

        if (it != InputType.NONE)
        {
            GameManager.GetInstance().ReceiveInput(it);
        }
    }
    
}
