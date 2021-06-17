using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Script used to set the button to a fixed sprite.
/// 
/// </summary>
public class ButtonToggleSprite : MonoBehaviour
{
    // Variables
    Sprite _unPressed;      // Unpressed sprite to set
    Sprite _pressed;        // Pressed sprite to set 
    Button _button;         // Button component

    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(TaskOnClick);

        _unPressed = _button.GetComponent<Image>().sprite;

        try
        {
            _pressed = _button.spriteState.pressedSprite;
        } // try
        catch(Exception e)
        {
            Debug.LogError("Please, add some sprite to the button pressed option." + e);
        } // catch
    } // Start

    /// <summary>
    /// 
    /// Task to do when the button is clicked.
    /// 
    /// </summary>
    public void TaskOnClick()
    {
        if(_button.GetComponent<Image>().sprite == _unPressed)
        {
            _button.GetComponent<Image>().sprite = _pressed;
        } // if
        else
        {
            _button.GetComponent<Image>().sprite = _unPressed;
        } // else
    } // if

    /// <summary>
    /// 
    /// Checks if the button is pressed or not.
    /// 
    /// </summary>
    /// <returns> (bool) Whether is pressed or not. </returns>
    public bool IsPressed()
    {
        return _button.GetComponent<Image>().sprite == _pressed;
    } // IsPressed
} // ButtonToggleSprite
