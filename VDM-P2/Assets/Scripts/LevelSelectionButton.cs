using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// Class that will control the level selection buttons.
/// 
/// </summary>
public class LevelSelectionButton : MonoBehaviour
{
    [Header("Color configuration")]
    public Color _textColor;           // Color of the text
    public Color _lockColor;           // Color of the lock button
     
    // Button Management
    Image _sprite;                     // Sprite to use
    GameObject _lText;                 // Level text
    GameObject _lock;                  // Lock image
    Color _color;                      // Color to use
    int _level;                        // Level that will initiate
    bool _completed;                   // Check if level is complete

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
    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        // This GameObject Button component
        Button bt = gameObject.GetComponent<Button>();

        // Adds the listener function
        bt.onClick.AddListener(TaskOnClick);
    } // Start

    /// <summary>
    /// 
    /// What the button will do when clicked.
    /// 
    /// </summary>
    public void TaskOnClick()
    {
        GameManager.GetInstance().SetLevel(_level - 1);
        GameManager.GetInstance().ChangeToLevel();
    } // TaskOnClick

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

    /// <summary>
    /// 
    /// Set if the level is complete or not. 
    /// 
    /// </summary>
    /// <param name="c"></param>
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

    /// <summary>
    /// 
    /// Set if the button will be interactable or not.
    /// 
    /// </summary>
    /// <param name="i"> (bool) State. </param>
    public void SetInteractable(bool i){
        this.gameObject.GetComponent<Button>().interactable = i;

        UpdateButton();
    } // SetIntactable
} // LevelSelectionButton