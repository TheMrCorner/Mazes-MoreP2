using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    [Header("Color configuration")]
    public Color _textColor;
    public Color _lockColor;

    // Button Management
    Image _sprite;
    GameObject _lText;
    GameObject _lock;
    Color _color;
    int _level;
    bool _completed;

    private void Awake()
    {
        _sprite = this.GetComponent<Image>(); // Get Button sprite for color change

        // Get children for updating button
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.GetComponent<Text>())
            {
                _lText = child.gameObject;
            } // if
            else if (child.gameObject.GetComponent<Image>())
            {
                _lock = child.gameObject;
            } // else if
        } // foreach
    }

    // Start is called before the first frame update
    void Start()
    {
        // This GameObject Button component
        Button bt = gameObject.GetComponent<Button>();

        // Adds the listener function
        bt.onClick.AddListener(TaskOnClick);
    } // Start

    public void TaskOnClick()
    {
        //GameManager.GetInstance().InitLevel(_level);
        Debug.Log("Pressed button: " + _level);
    }

    /// <summary>
    /// 
    /// Updates the visual part of the button, depending on interactable
    /// status and completed status.
    /// 
    /// </summary>
    void UpdateButton()
    {
        // Check if object is interactable
        if(this.gameObject.GetComponent<Button>().interactable)
        {
            _lText.SetActive(true);
            _lock.SetActive(false);
            // Then check if level is completed
            if (_completed)
            {
                _lText.GetComponent<Text>().color = Color.white;
                _sprite.color = _color;
            } // if
            else
            {
                _lText.GetComponent<Text>().color = _textColor;
                _sprite.color = Color.white;
            } // else
        } // if
        else
        {
            _lText.SetActive(false);
            _lock.SetActive(true);
            _sprite.color = _lockColor;
        } // else
    } // UpdateButton

    /// <summary>
    /// 
    /// Function to set the level number in the text of the button. It
    /// also stores the level number to be loaded in the GameScene.
    /// 
    /// </summary>
    /// <param name="l"> (int) Number of the level. </param>
    public void SetLevel(int l)
    {
        _level = l;

        _lText.GetComponent<Text>().text = (_level).ToString();
    } // SetLevel

    public void SetCompleted(bool c)
    {
        _completed = c;
    } // SetCompleted

    /// <summary>
    /// 
    /// Set the color that will be displayed when the player has completed
    /// the level.
    /// 
    /// </summary>
    /// <param name="c"> (Color) Color to put the button when interactable and completed. </param>
    public void SetColor(Color c)
    {
        _color = c;
    } // SetColor

    public void SetInteractable(bool i){
        this.gameObject.GetComponent<Button>().interactable = i;

        UpdateButton();
    } // SetIntactable
} // LevelSelectionButton