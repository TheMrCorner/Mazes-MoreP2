using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggleSprite : MonoBehaviour
{
    // Variables
    Sprite _unPressed;
    Sprite _pressed;
    Button _button;

    // Start is called before the first frame update
    void Start()
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
            Debug.LogError("Pls, add some sprite to the button pressed option." + e);
        } // catch
    } // Start

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

    public bool IsPressed()
    {
        return _button.GetComponent<Image>().sprite == _pressed;
    }
} // ButtonToggleSprite
