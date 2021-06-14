using UnityEngine;
using UnityEngine.UI;

public class OnOffButton : MonoBehaviour
{
    public Text _onText;
    public Text _offText;
    public int _alphaValue;

    public void TaskOnClick()
    {
        if(_onText.color.a == 255)
        {
            Color c = _onText.color;
            c.a = _alphaValue;
            _onText.color = c;

            c = _offText.color;
            c.a = 255;
            _offText.color = c;

            // Notify gameManager
        } // if
        else
        {
            Color c = _offText.color;
            c.a = _alphaValue;
            _offText.color = c;

            c = _onText.color;
            c.a = 255;
            _onText.color = c;

            // Notify GameManager
        } // else
    } // TaskOnClick
} // OnOffButton
